angular.module('ngApp.loginview', [
  'ui.router',
  'ngApp.common',
  'ui.grid',
  'ui.grid.resizeColumns'
])

.config(function config($stateProvider, $locationProvider) {
    //$locationProvider.html5Mode(true);


    $stateProvider
        // -- Master-Admin State Start --

        .state('msadmin', {
            abstract: true,
            url: '/msadmin',
            controller: 'LoginViewController',
            templateUrl: 'loginView/loginView.tpl.html'
        })

          .state('msadmin.customers', {
              url: '/customers',
              templateUrl: 'customer/customer.tpl.html',
              controller: 'CustomerController',
              data: { pageTitle: 'Customers' }
          })

        .state('msadmin.customer-detail', {
            abstract: true,
            url: '/customer-detail/:customerId',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerBasicController',
            data: { pageTitle: 'Customer Detail' }
        })
          .state('msadmin.customer-detail.basic-detail', {
              url: '/basic-detail',
              templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
              controller: 'CustomerBasicDetailController',
              data: { pageTitle: 'Customer Basic Detail' }
          })
          .state('msadmin.customer-detail.margincost', {
              url: '/margincost',
              templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
              controller: 'CustomerMarginCostController',
              data: { pageTitle: 'Customer Margin Cost' }
          })
         .state('msadmin.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer Advance Rate Card' }
         })
            .state('msadmin.customer-detail.customerRateCard', {
                url: '/customerRateCard',
                templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
                controller: 'CustomerRateCardSettingController',
                data: { pageTitle: 'Customer Rate Card' }
            })
           .state('msadmin.courier-accounts', {
               url: '/courier-accounts',
               abstract: true,
               templateUrl: 'courierAccount/countryAccount.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('msadmin.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('msadmin.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
         .state('msadmin.zone-setting', {
             abstract: true,
             url: '/zone-setting',
             templateUrl: 'zoneSetting/zoneSetting.tpl.html',
             controller: 'ZoneSettingController',
             data: { pageTitle: 'Zone Setting' }
         })
         .state('msadmin.zone-setting.zone-country', {
             url: '/zone-country',
             templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
             controller: 'ZoneCountryController',
             data: { pageTitle: 'Country Zone' }
         })
          .state('msadmin.zone-setting.third-party-matrix', {
              url: '/third-party-matrix',
              templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
              controller: 'ThirdPartyMatrixController',
              data: { pageTitle: '3rd Party Matrix' }
          })
          .state('msadmin.zone-setting.base-rate-card', {
              url: '/base-rate-card',
              templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
              controller: 'BaseRateCardController',
              data: { pageTitle: ' Base Rate Card' }
          })
          .state('msadmin.zone-setting.zone-postCode', {
              url: '/zone-postCode',
              templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
              controller: 'zonePostCodeController',
              data: { pageTitle: ' Zone Post/Zip Code' }
          })
        .state('msadmin.zone-setting.countryzone-postCode', {
            url: '/countryzone-postCode',
            templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
            controller: 'CountryZonePostCodeController',
            data: { pageTitle: ' Country Zone Post/Zip Code' }
        })
                 .state('msadmin.booking-home.address-book', {
                     url: '/address-book/:userId',
                     templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
                     controller: 'CustomerAddressBookController',
                     data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
                 })
             .state('msadmin.manifests', {
                 url: '/manifests',
                 templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
                 controller: 'CustomerManifestController',
                 data: { pageTitle: 'Manifests' }
             })
         .state('msadmin.manifests.user-manifests', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('msadmin.manifests.custom-manifests', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
          .state('msadmin.user', {
              url: '/user',
              templateUrl: 'user/user.tpl.html',
              controller: 'UserController',
              data: { pageTitle: 'Users' }
          })

        .state('msadmin.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })

          //Direct Booking Routing
         .state('msadmin.booking-home', {
             url: '/booking-home',
             abstract: true,
             templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
             //  controller: 'BookingHomeController',
             data: { pageTitle: 'Booking Home' }
         })
        .state('msadmin.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('msadmin.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
          .state('msadmin.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
         .state('msadmin.booking-home.direct-booking-return', {
             url: '/direct-booking-return/:directShipmentId',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
         })

        // Track And Trace
         .state('msadmin.direct-shipments', {
             url: '/direct-shipments',
             templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
             controller: 'DirectShipmentController',
             data: { pageTitle: 'Track & Trace' }
         })

        // Setting Routing
       .state('msadmin.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('msadmin.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('msadmin.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('msadmin.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('msadmin.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('msadmin.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('msadmin.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('msadmin.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('msadmin.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('msadmin.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })

           // Access-Level
         .state('msadmin.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('msadmin.hs-code-operation', {
            url: '/hs-code-operation',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })

         .state('msadmin.hscode-jobs-dashboard', {
             url: '/hscode-jobs-dashboard',
             templateUrl: 'jobs/jobsDashboard.tpl.html',
             controller: 'JobsDashboradController',
             data: { pageTitle: 'Jobs DasdhBoard' }
         })

         .state('msadmin.quotation-tools', {
             url: '/quotation-tools',
             templateUrl: 'quotationTools/quotationTools.tpl.html',
             controller: 'QuotationToolController',
             data: { pageTitle: 'Quotation' }
         })

        // -- Master-Admin State End --

        // -- Admin State Start --
        .state('admin', {
            abstract: true,
            url: '/admin',
            controller: 'LoginViewController',
            templateUrl: 'loginView/loginView.tpl.html'
        })

        .state('admin.customers', {
            url: '/customers',
            templateUrl: 'customer/customer.tpl.html',
            controller: 'CustomerController',
            data: { pageTitle: 'Customers' }
        })

        .state('admin.customer-detail', {
            abstract: true,
            url: '/customer-detail/:customerId',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerBasicController',
            data: { pageTitle: 'Customer Detail' }
        })
          .state('admin.customer-detail.basic-detail', {
              url: '/basic-detail',
              templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
              controller: 'CustomerBasicDetailController',
              data: { pageTitle: 'Customer Basic Detail' }
          })
          .state('admin.customer-detail.margincost', {
              url: '/margincost',
              templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
              controller: 'CustomerMarginCostController',
              data: { pageTitle: 'Customer Margin Cost' }
          })
      .state('admin.customer-detail.advanceratecard', {
          url: '/advanceratecard',
          templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
          controller: 'CustomerAdvanceRateCardController',
          data: { pageTitle: 'Customer Advance Rate Card' }
      })
        .state('admin.customer-detail.customerRateCard', {
            url: '/customerRateCard',
            templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
            controller: 'CustomerRateCardSettingController',
            data: { pageTitle: 'Customer Rate Card' }
        })
          .state('admin.receivers', {
              url: '/receivers',
              templateUrl: 'receiver/receiver.tpl.html',
              controller: 'ReceiverController',
              data: { pageTitle: 'Receivers' }
          })
        .state('admin.receiver-detail', {
            url: '/receiver-detail/:receiverId',
            templateUrl: 'shipper/shipperAddEdit.tpl.html',
            controller: 'ShipperAddEditController',
            data: { pageTitle: 'Receiver Detail' }
        })

        .state('admin.shippers', {
            url: '/shippers',
            templateUrl: 'shipper/shipper.tpl.html',
            controller: 'ShipperController',
            data: { pageTitle: 'Shipper' }
        })

        .state('admin.shippers-detail', {
            url: '/shippers-detail/:shipperId',
            templateUrl: 'shipper/shipperAddEdit.tpl.html',
            controller: 'ShipperAddEditController',
            data: { pageTitle: 'Shipper Detail' }
        })

        .state('admin.current-shipment', {
            url: '/current-shipment',
            templateUrl: 'shipment/currentShipment/currentShipment.tpl.html',
            controller: 'CurrentShipmentController',
            data: { pageTitle: 'Current Shipment' }
        })

        .state('admin.past-shipment', {
            url: '/past-shipment',
            templateUrl: 'shipment/pastShipment/pastShipment.tpl.html',
            controller: 'PastShipmentController',
            data: { pageTitle: 'Past Shipment' }
        })

        .state('admin.agents', {
            url: '/agents',
            templateUrl: 'agent/agent.tpl.html',
            controller: 'AgentController',
            data: { pageTitle: 'Agents' }
        })

         .state('admin.agent-detail', {
             url: '/agent-detail/:AgentId',
             templateUrl: 'agent/agentDetail.tpl.html',
             controller: 'AgentDetailController',
             data: { pageTitle: 'Agents Detail' }
         })

         .state('admin.shipping-method', {
             url: '/shipping-method',
             templateUrl: 'courier/courier.tpl.html',
             controller: 'CourierController',
             data: { pageTitle: 'Shipment Methods' }
         })

        .state('admin.country', {
            url: '/country',
            templateUrl: 'country/country.tpl.html',
            controller: 'CountryController',
            data: { pageTitle: 'Countries' }
        })

        .state('admin.country-detail', {
            url: '/country-detail/:CountryId',
            templateUrl: 'country/countryDetail/country-detail.tpl.html',
            controller: 'CountryDetailController',
            //controller: 'CountryAddEditController',
            data: { pageTitle: 'Countries' }
        })

        .state('admin.timezone', {
            url: '/timezone',
            templateUrl: 'timezone/timezone.tpl.html',
            controller: 'TimeZoneController',
            data: { pageTitle: 'Time Zone' }
        })

        .state('admin.warehouse', {
            url: '/warehouse',
            templateUrl: 'warehouse/warehouse.tpl.html',
            controller: 'WarehouseController',
            data: { pageTitle: 'Warehouse' }
        })
        .state('admin.warehouse-detail', {
            url: '/warehouse-detail/:warehouseId',
            templateUrl: 'warehouse/warehouse-detail.tpl.html',
            controller: 'WarehouseAddEditController',
            data: { pageTitle: 'Warehouse-Detail ' }

        })

        .state('admin.carrier', {
            url: '/carrier',
            templateUrl: 'carrier/carrier.tpl.html',
            controller: 'CarrierController',
            data: { pageTitle: 'Carrier' }
        })

         .state('admin.user', {
             url: '/user',
             templateUrl: 'user/user.tpl.html',
             controller: 'UserController',
             data: { pageTitle: 'Users' }
         })

        .state('admin.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })

         .state('admin.tradelane', {
             url: '/tradelane',
             templateUrl: 'tradelane/tradelane.tpl.html',
             controller: 'TradelaneController',
             data: { pageTitle: 'Trade Lane' }
         })

        .state('admin.weekdays', {
            url: '/weekdays',
            templateUrl: 'weekDays/weekDays.tpl.html',
            controller: 'WeekDaysController',
            data: { pageTitle: 'Week Days' }
        })


        // Direct Booking Routing
            .state('admin.booking-home', {
                url: '/booking-home',
                abstract: true,
                templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
                //  controller: 'BookingHomeController',
                data: { pageTitle: 'Booking Home' }
            })
        .state('admin.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('admin.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
          .state('admin.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
         .state('admin.booking-home.direct-booking-return', {
             url: '/direct-booking-return/:directShipmentId',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
         })
         .state('admin.direct-shipments', {
             url: '/direct-shipments',
             templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
             controller: 'DirectShipmentController',
             data: { pageTitle: 'Track & Trace' }
         })

        .state('admin.courier-accounts', {
            url: '/courier-accounts',
            abstract: true,
            templateUrl: 'courierAccount/countryAccount.tpl.html',
            controller: 'CourierAccountController',
            data: { pageTitle: 'Couriers Account' }
        })
           .state('admin.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('admin.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
         .state('admin.zone-setting', {
             abstract: true,
             url: '/zone-setting',
             templateUrl: 'zoneSetting/zoneSetting.tpl.html',
             controller: 'ZoneSettingController',
             data: { pageTitle: 'Zone Setting' }
         })
         .state('admin.zone-setting.zone-country', {
             url: '/zone-country',
             templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
             controller: 'ZoneCountryController',
             data: { pageTitle: 'Country Zone' }
         })
          .state('admin.zone-setting.third-party-matrix', {
              url: '/third-party-matrix',
              templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
              controller: 'ThirdPartyMatrixController',
              data: { pageTitle: '3rd Party Matrix' }
          })
          .state('admin.zone-setting.base-rate-card', {
              url: '/base-rate-card',
              templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
              controller: 'BaseRateCardController',
              data: { pageTitle: ' Base Rate Card' }
          })
          .state('admin.zone-setting.zone-postCode', {
              url: '/zone-postCode',
              templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
              controller: 'zonePostCodeController',
              data: { pageTitle: ' Zone Post/Zip Code' }
          })
           .state('admin.zone-setting.countryzone-postCode', {
               url: '/countryzone-postCode',
               templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
               controller: 'CountryZonePostCodeController',
               data: { pageTitle: ' Country Zone Post/Zip Code' }
           })
        .state('admin.setting', {
            abstract: true,
            url: '/setting',
            templateUrl: 'setting/setting.tpl.html',
            controller: 'SettingController',
            data: { pageTitle: 'Setting' }
        })
        .state('admin.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('admin.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('admin.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })

        .state('admin.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('admin.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('admin.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('admin.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('admin.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('admin.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })
        .state('admin.quotation-tools', {
            url: '/quotation-tools',
            templateUrl: 'quotationTools/quotationTools.tpl.html',
            controller: 'QuotationToolController',
            data: { pageTitle: 'Quotation' }
        })
              .state('admin.booking-home.address-book', {
                  url: '/address-book/:userId',
                  templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
                  controller: 'CustomerAddressBookController',
                  data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
              })
              .state('admin.manifests', {
                  url: '/manifests',
                  templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
                  controller: 'CustomerManifestController',
                  data: { pageTitle: 'Manifests' }
              })
         .state('admin.manifests.user-manifests', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('admin.manifests.custom-manifests', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
                   // Access-Level
         .state('admin.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('admin.hs-code-operation', {
            url: '/hs-code-operation',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })
           .state('admin.hscode-jobs-dashboard', {
               url: '/hscode-jobs-dashboard',
               templateUrl: 'jobs/jobsDashboard.tpl.html',
               controller: 'JobsDashboradController',
               data: { pageTitle: 'Jobs DasdhBoard' }
           })

    // -- Admin State End

    // -- Shipper State Start --

        .state('shipper', {
            abstract: true,
            url: '/shipper',
            controller: 'LoginViewController',
            templateUrl: 'loginView/loginView.tpl.html'
        })

        .state('shipper.current-shipment', {
            url: '/current-shipment',
            templateUrl: 'shipment/currentShipment/currentShipment.tpl.html',
            controller: 'CurrentShipmentController',
            data: { pageTitle: 'Current Shipment' }
        })

        .state('shipper.past-shipment', {
            url: '/past-shipment',
            templateUrl: 'shipment/pastShipment/pastShipment.tpl.html',
            controller: 'PastShipmentController',
            data: { pageTitle: 'Past Shipment' }
        })

         .state('shipper.receivers', {
             url: '/receivers',
             templateUrl: 'receiver/receiver.tpl.html',
             controller: 'ReceiverController',
             data: { pageTitle: 'Receivers' }
         })

        .state('shipper.receiver-detail', {
            url: '/receiver-detail/:receiverId',
            templateUrl: 'shipper/shipperAddEdit.tpl.html',
            controller: 'ShipperAddEditController',
            data: { pageTitle: 'Receiver Detail' }
        })

        .state('shipper.manage-detail', {
            url: '/manage-detail',
            templateUrl: 'shipper/shipperAddEdit.tpl.html',
            controller: 'ShipperAddEditController',
            data: { pageTitle: 'Manage Detail' }
        })

        .state('shipper.tools', {
            url: '/tools',
            templateUrl: 'shipper/tools.tpl.html',
            //controller: 'ShipperAddEditController',
            data: { pageTitle: 'Tools' }
        })

        .state('shipper.shipment', {
            abstract: true,
            url: '/shipment/:Id/:UserType',
            controller: 'ShipmentController',
            templateUrl: 'shipment/shipment.tpl.html'
        })

         .state('shipper.shipment.addressdetail', {
             url: '/addressdetail',
             templateUrl: 'shipment/addressdetail.tpl.html',
             data: { pageTitle: 'Shipment address details' }
         })
           .state('shipper.shipment.addreceiver', {
               url: '/receiver-detail/:shipperId/:receiverId',
               templateUrl: 'shipper/shipperAddEdit.tpl.html',
               controller: 'ShipperAddEditController',
               data: { pageTitle: 'Shipment address details' }
           })

        .state('shipper.shipment.shipmentdetail', {
            url: '/shipmentdetail',
            templateUrl: 'shipment/shipmentdetail.tpl.html',
            data: { pageTitle: 'Shipment details' }
        })

        .state('shipper.shipment.serviceoption', {
            url: '/serviceoption',
            templateUrl: 'shipment/serviceoption.tpl.html',
            data: { pageTitle: 'Shipment service option' }
        })

        .state('shipper.shipment.confirmshipment', {
            url: '/confirmshipment',
            templateUrl: 'shipment/confirmshipment.tpl.html',
            data: { pageTitle: 'Confirm shipment' }
        })

    // -- Shipper State End --

    // -- Customer State Start --

     .state('customer', {
         abstract: true,
         url: '/customer',
         controller: 'LoginViewController',
         templateUrl: 'loginView/loginView.tpl.html'
     })

            .state('customer.customers', {
                url: '/customers',
                templateUrl: 'customer/customer.tpl.html',
                controller: 'CustomerController',
                data: { pageTitle: 'Customers' }
            })

        .state('customer.customer-detail', {
            abstract: true,
            url: '/customer-detail/:customerId',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerBasicController',
            data: { pageTitle: 'Customer Detail' }
        })
          .state('customer.customer-detail.basic-detail', {
              url: '/basic-detail',
              templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
              controller: 'CustomerBasicDetailController',
              data: { pageTitle: 'Customer Basic Detail' }
          })
          .state('customer.customer-detail.margincost', {
              url: '/margincost',
              templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
              controller: 'CustomerMarginCostController',
              data: { pageTitle: 'Customer MarginCost' }
          })
         .state('customer.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer AdvanceRateCard' }
         })
            .state('customer.customer-detail.customerRateCard', {
                url: '/customerRateCard',
                templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
                controller: 'CustomerRateCardSettingController',
                data: { pageTitle: 'Customer RateCard' }
            })
           .state('customer.courier-accounts', {
               url: '/courier-accounts',
               abstract: true,
               templateUrl: 'courierAccount/countryAccount.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('customer.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('customer.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
         .state('customer.zone-setting', {
             abstract: true,
             url: '/zone-setting',
             templateUrl: 'zoneSetting/zoneSetting.tpl.html',
             controller: 'ZoneSettingController',
             data: { pageTitle: 'Zone Setting' }
         })
         .state('customer.zone-setting.zone-country', {
             url: '/zone-country',
             templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
             controller: 'ZoneCountryController',
             data: { pageTitle: 'Country Zone' }
         })
          .state('customer.zone-setting.third-party-matrix', {
              url: '/third-party-matrix',
              templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
              controller: 'ThirdPartyMatrixController',
              data: { pageTitle: '3rd Party Matrix' }
          })
          .state('customer.zone-setting.base-rate-card', {
              url: '/base-rate-card',
              templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
              controller: 'BaseRateCardController',
              data: { pageTitle: ' Base Rate Card' }
          })
          .state('customer.zone-setting.zone-postCode', {
              url: '/zone-postCode',
              templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
              controller: 'zonePostCodeController',
              data: { pageTitle: ' Zone Post/Zip Code' }
          })
         .state('customer.zone-setting.countryzone-postCode', {
             url: '/countryzone-postCode',
             templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
             controller: 'CountryZonePostCodeController',
             data: { pageTitle: ' Country Zone Post/Zip Code' }
         })
          .state('customer.user', {
              url: '/user',
              templateUrl: 'user/user.tpl.html',
              controller: 'UserController',
              data: { pageTitle: 'Users' }
          })

        .state('customer.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })


  // Setting Routing
       .state('customer.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('customer.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('customer.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('customer.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('customer.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('customer.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('customer.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('customer.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('customer.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('customer.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })

           // Access-Level
         .state('customer.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('customer.hs-code-operation', {
            url: '/hs-code-operation',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })

             .state('customer.hscode-jobs-dashboard', {
                 url: '/hscode-jobs-dashboard',
                 templateUrl: 'jobs/jobsDashboard.tpl.html',
                 controller: 'JobsDashboradController',
                 data: { pageTitle: 'Jobs DasdhBoard' }
             })
        // Customer Direct Booking

        .state('customer.booking-home', {
            url: '/booking-home',
            abstract: true,
            templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
            //  controller: 'BookingHomeController',
            data: { pageTitle: 'Booking Home' }
        })
        .state('customer.booking-home.direct-booking', {
            url: '/direct-booking/:directShipmentId/:callingtype',
            templateUrl: 'directBooking/directBooking.tpl.html',
            controller: 'DirectBookingController',
            data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
        })
        .state('customer.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
        //.state('customer.setting', {
        //    url: '/setting',
        //    templateUrl: 'customer/customerSetting/customerSetting.tpl.html',
        //    controller: 'CustomerSettingController',
        //    data: { pageTitle: 'Customer Setting' }
        //})
        .state('customer.direct-shipments', {
            url: '/direct-shipments/:moduleType',
            templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
            controller: 'DirectShipmentController',
            data: { pageTitle: 'Track & Trace' }
        })
         .state('customer.booking-home.address-book', {
             url: '/address-book/:userId',
             templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
             controller: 'CustomerAddressBookController',
             data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
         })
           
          .state('customer.manifest', {
              url: '/manifests',
              abstract: true,
              templateUrl: 'customer/customerManifest/manifest.tpl.html',
              controller: 'ManifestController',
              data: { pageTitle: 'Manifests' }
          })
         .state('customer.manifest.userManifest', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/userManifest/customerManifests.tpl.html',
             controller: 'UserManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('customer.manifest.customManifest', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customManifest/customerManifests.tpl.html',
              controller: 'CustomManifestController',
              data: { pageTitle: 'Manifests' }
          })

        .state('customer.booking-home.direct-booking-clone', {
            url: '/direct-booking-clone/:directShipmentId',
            templateUrl: 'directBooking/directBooking.tpl.html',
            controller: 'DirectBookingController',
            data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
        })
          .state('customer.booking-home.direct-booking-return', {
              url: '/direct-booking-return/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
          })
        .state('customer.quotation-tools', {
            url: '/quotation-tools',
            templateUrl: 'quotationTools/quotationTools.tpl.html',
            controller: 'QuotationToolController',
            data: { pageTitle: 'Quotation' }
        })

        // Customer eCommerce 
         .state('customer.booking-home.eCommerce-booking', {
             url: '/eCommerce-booking/:shipmentId',
             templateUrl: 'eCommerceBooking/eCommerceBooking.tpl.html',
             controller: 'eCommerceBookingController',
             data: { pageTitle: 'eCommerce Booking', IsFired: true, IsChanged: false }
         })


        .state('customer.booking-home.eCommerce-booking-clone', {
            url: '/eCommerce-booking-clone/:shipmentId',
            templateUrl: 'eCommerceBooking/eCommerceBooking.tpl.html',
            controller: 'eCommerceBookingController',
            data: { pageTitle: 'eCommerce Booking Clone', IsFired: true, IsChanged: false }
        })
          .state('customer.booking-home.eCommerce-booking-return', {
              url: '/eCommerce-booking-return/:shipmentId',
              templateUrl: 'eCommerceBooking/eCommerceBooking.tpl.html',
              controller: 'eCommerceBookingController',
              data: { pageTitle: 'eCommerce Booking Return', IsFired: true, IsChanged: false }
          })
        //
        .state('customer.current-shipment', {
            url: '/current-shipment',
            templateUrl: 'shipment/currentShipment/currentShipment.tpl.html',
            controller: 'CurrentShipmentController',
            data: { pageTitle: 'Current Shipment' }
        })

        .state('customer.past-shipment', {
            url: '/past-shipment',
            templateUrl: 'shipment/pastShipment/pastShipment.tpl.html',
            controller: 'PastShipmentController',
            data: { pageTitle: 'Past Shipment' }
        })

         .state('customer.receivers', {
             url: '/receivers/:receiverId',
             templateUrl: 'receiver/receiver.tpl.html',
             controller: 'ReceiverController',
             data: { pageTitle: 'Receivers' }
         })

         .state('customer.receiver-detail', {
             url: '/receiver-detail/:receiverId',
             templateUrl: 'shipper/shipperAddEdit.tpl.html',
             controller: 'ShipperAddEditController',
             data: { pageTitle: 'Receiver Detail' }
         })

        .state('customer.manage-detail', {
            url: '/manage-detail',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerDetailController',
            data: { pageTitle: 'Manage Detail' }
        })


    // -- Customer State End --

    // -- Staff user State Start --

    //.state('user', {
    //    abstract: true,
    //    url: '/user',
    //    controller: 'LoginViewController',
    //    templateUrl: 'loginView/loginView.tpl.html'
    //})

    //    .state('user.current-shipment', {
    //        url: '/current-shipment',
    //        templateUrl: 'shipment/currentShipment/currentShipment.tpl.html',
    //        controller: 'CurrentShipmentController',
    //        data: { pageTitle: 'Current Shipment' }
    //    })

    //    .state('user.past-shipment', {
    //        url: '/past-shipment',
    //        templateUrl: 'shipment/pastShipment/pastShipment.tpl.html',
    //        controller: 'PastShipmentController',
    //        data: { pageTitle: 'Past Shipment' }
    //    })

    //    .state('user.manage-detail', {
    //        url: '/manage-detail',
    //        templateUrl: 'user/userDetail.tpl.html',
    //        controller: 'UserDetailController',
    //        data: { pageTitle: 'Manage Detail' }
    //    })
    // .state('user.build-consignment', {
    //     url: '/build-consignment',
    //     templateUrl: 'expryes/buildConsignment/buildConsignment.tpl.html',
    //     controller: 'BuildConsignmentController',
    //     data: { pageTitle: 'Build Consigment' }
    // })
    //.state('user.tracing', {
    //    url: '/tracing',
    //    templateUrl: 'expryes/tracingDetail/tracingDetail.tpl.html',
    //    controller: 'TracingDetailController',
    //    data: { pageTitle: 'Tracing Details' }
    //})

    // -- Staff user State End --

    //-- Direct Booking User Start --

    .state('dbuser', {
        abstract: true,
        url: '/dbuser',
        controller: 'LoginViewController',
        templateUrl: 'loginView/loginView.tpl.html'
    })

    .state('dbuser.customers', {
        url: '/customers',
        templateUrl: 'customer/customer.tpl.html',
        controller: 'CustomerController',
        data: { pageTitle: 'Customers' }
    })

    .state('dbuser.customer-detail', {
        abstract: true,
        url: '/customer-detail/:customerId',
        templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
        controller: 'CustomerBasicController',
        data: { pageTitle: 'Customer Detail' }
    })
      .state('dbuser.customer-detail.basic-detail', {
          url: '/basic-detail',
          templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
          controller: 'CustomerBasicDetailController',
          data: { pageTitle: 'Customer Basic Detail' }
      })
      .state('dbuser.customer-detail.margincost', {
          url: '/margincost',
          templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
          controller: 'CustomerMarginCostController',
          data: { pageTitle: 'Customer MarginCost' }
      })
         .state('dbuser.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer AdvanceRateCard' }
         })
        .state('dbuser.customer-detail.customerRateCard', {
            url: '/customerRateCard',
            templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
            controller: 'CustomerRateCardSettingController',
            data: { pageTitle: 'Customer RateCard' }
        })
     .state('dbuser.shipping-method', {
         url: '/shipping-method',
         templateUrl: 'courier/courier.tpl.html',
         controller: 'CourierController',
         data: { pageTitle: 'Shipment Methods' }
     })

     .state('dbuser.direct-shipments', {
         url: '/direct-shipments',
         templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
         controller: 'DirectShipmentController',
         data: { pageTitle: 'Direct Shipments' }
     })
              .state('dbuser.booking-home', {
                  url: '/booking-home',
                  abstract: true,
                  templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
                  //  controller: 'BookingHomeController',
                  data: { pageTitle: 'Booking Home' }
              })
        .state('dbuser.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('dbuser.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
           .state('dbuser.booking-home.direct-booking-return', {
               url: '/direct-booking-return/:directShipmentId',
               templateUrl: 'directBooking/directBooking.tpl.html',
               controller: 'DirectBookingController',
               data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
           })
          .state('dbuser.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
    .state('dbuser.booking-home.address-book', {
        url: '/address-book/:userId',
        templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
        controller: 'CustomerAddressBookController',
        data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
    })
            
        .state('dbuser.manifests', {
            url: '/manifests',
            templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
            controller: 'CustomerManifestController',
            data: { pageTitle: 'Manifests' }
        })
         .state('dbuser.manifest.userManifest', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('dbuser.manifests.customManifest', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
     .state('dbuser.zone-setting', {
         abstract: true,
         url: '/zone-setting',
         templateUrl: 'zoneSetting/zoneSetting.tpl.html',
         controller: 'ZoneSettingController',
         data: { pageTitle: 'Zone Setting' }
     })
     .state('dbuser.zone-setting.zone-country', {
         url: '/zone-country',
         templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
         controller: 'ZoneCountryController',
         data: { pageTitle: 'Country Zone' }
     })
      .state('dbuser.zone-setting.third-party-matrix', {
          url: '/third-party-matrix',
          templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
          controller: 'ThirdPartyMatrixController',
          data: { pageTitle: '3rd Party Matrix' }
      })
      .state('dbuser.zone-setting.base-rate-card', {
          url: '/base-rate-card',
          templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
          controller: 'BaseRateCardController',
          data: { pageTitle: ' Base Rate Card' }
      })
      .state('dbuser.zone-setting.zone-postCode', {
          url: '/zone-postCode',
          templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
          controller: 'zonePostCodeController',
          data: { pageTitle: ' Zone Post/Zip Code' }
      })
         .state('dbuser.zone-setting.countryzone-postCode', {
             url: '/countryzone-postCode',
             templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
             controller: 'CountryZonePostCodeController',
             data: { pageTitle: ' Country Zone Post/Zip Code' }
         })
       .state('dbuser.quotation-tools', {
           url: '/quotation-tools',
           templateUrl: 'quotationTools/quotationTools.tpl.html',
           controller: 'QuotationToolController',
           data: { pageTitle: 'Quotation' }
       })
            .state('dbuser.user', {
                url: '/user',
                templateUrl: 'user/user.tpl.html',
                controller: 'UserController',
                data: { pageTitle: 'Users' }
            })

        .state('dbuser.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })

          // Setting Routing
       .state('dbuser.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('dbuser.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('dbuser.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('dbuser.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('dbuser.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('dbuser.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('dbuser.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('dbuser.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('dbuser.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('dbuser.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })

           // Access-Level
         .state('dbuser.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('dbuser.hs-code-operation', {
            url: '/hs-code-operation',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })
               .state('dbuser.hscode-jobs-dashboard', {
                   url: '/hscode-jobs-dashboard',
                   templateUrl: 'jobs/jobsDashboard.tpl.html',
                   controller: 'JobsDashboradController',
                   data: { pageTitle: 'Jobs DasdhBoard' }
               })
    //-- Direct Booking User End --
         .state('dbuser.courier-accounts', {
             url: '/courier-accounts',
             abstract: true,
             templateUrl: 'courierAccount/countryAccount.tpl.html',
             controller: 'CourierAccountController',
             data: { pageTitle: 'Couriers Account' }
         })
           .state('dbuser.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('dbuser.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })


        // HSCode Operator Routing 

         .state('hsuser', {
             abstract: true,
             url: '/hsuser',
             controller: 'LoginViewController',
             templateUrl: 'loginView/loginView.tpl.html'
         })

        //Direct Booking Routing
         .state('hsuser.booking-home', {
             url: '/booking-home',
             abstract: true,
             templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
             //  controller: 'BookingHomeController',
             data: { pageTitle: 'Booking Home' }
         })
        .state('hsuser.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('hsuser.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
          .state('hsuser.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
         .state('hsuser.booking-home.direct-booking-return', {
             url: '/direct-booking-return/:directShipmentId',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
         })
        .state('hsuser.booking-home.address-book', {
            url: '/address-book/:userId',
            templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
            controller: 'CustomerAddressBookController',
            data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
        })
            
         .state('hsuser.manifests', {
             url: '/manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
         .state('hsuser.manifests.user-manifests', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('hsuser.manifests.custom-manifests', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
        // Track And Trace
         .state('hsuser.direct-shipments', {
             url: '/direct-shipments',
             templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
             controller: 'DirectShipmentController',
             data: { pageTitle: 'Track & Trace' }
         })

        // Setting Routing
       .state('hsuser.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('hsuser.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('hsuser.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('hsuser.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('hsuser.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('hsuser.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('hsuser.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('hsuser.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('hsuser.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('hsuser.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })


             .state('hsuser.zone-setting', {
                 abstract: true,
                 url: '/zone-setting',
                 templateUrl: 'zoneSetting/zoneSetting.tpl.html',
                 controller: 'ZoneSettingController',
                 data: { pageTitle: 'Zone Setting' }
             })
     .state('hsuser.zone-setting.zone-country', {
         url: '/zone-country',
         templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
         controller: 'ZoneCountryController',
         data: { pageTitle: 'Country Zone' }
     })
      .state('hsuser.zone-setting.third-party-matrix', {
          url: '/third-party-matrix',
          templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
          controller: 'ThirdPartyMatrixController',
          data: { pageTitle: '3rd Party Matrix' }
      })
      .state('hsuser.zone-setting.base-rate-card', {
          url: '/base-rate-card',
          templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
          controller: 'BaseRateCardController',
          data: { pageTitle: ' Base Rate Card' }
      })
      .state('hsuser.zone-setting.zone-postCode', {
          url: '/zone-postCode',
          templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
          controller: 'zonePostCodeController',
          data: { pageTitle: ' Zone Post/Zip Code' }
      })
        .state('hsuser.zone-setting.countryzone-postCode', {
            url: '/countryzone-postCode',
            templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
            controller: 'CountryZonePostCodeController',
            data: { pageTitle: ' Country Zone Post/Zip Code' }
        })
       .state('hsuser.quotation-tools', {
           url: '/quotation-tools',
           templateUrl: 'quotationTools/quotationTools.tpl.html',
           controller: 'QuotationToolController',
           data: { pageTitle: 'Quotation' }
       })
            .state('hsuser.user', {
                url: '/user',
                templateUrl: 'user/user.tpl.html',
                controller: 'UserController',
                data: { pageTitle: 'Users' }
            })

        .state('hsuser.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })
          .state('hsuser.customers', {
              url: '/customers',
              templateUrl: 'customer/customer.tpl.html',
              controller: 'CustomerController',
              data: { pageTitle: 'Customers' }
          })

    .state('hsuser.customer-detail', {
        abstract: true,
        url: '/customer-detail/:customerId',
        templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
        controller: 'CustomerBasicController',
        data: { pageTitle: 'Customer Detail' }
    })
      .state('hsuser.customer-detail.basic-detail', {
          url: '/basic-detail',
          templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
          controller: 'CustomerBasicDetailController',
          data: { pageTitle: 'Customer Basic Detail' }
      })
      .state('hsuser.customer-detail.margincost', {
          url: '/margincost',
          templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
          controller: 'CustomerMarginCostController',
          data: { pageTitle: 'Customer MarginCost' }
      })
         .state('hsuser.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer AdvanceRateCard' }
         })
        .state('hsuser.customer-detail.customerRateCard', {
            url: '/customerRateCard',
            templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
            controller: 'CustomerRateCardSettingController',
            data: { pageTitle: 'Customer RateCard' }
        })
                 .state('hsuser.courier-accounts', {
                     url: '/courier-accounts',
                     abstract: true,
                     templateUrl: 'courierAccount/countryAccount.tpl.html',
                     controller: 'CourierAccountController',
                     data: { pageTitle: 'Couriers Account' }
                 })
           .state('hsuser.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('hsuser.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           // Access-Level
         .state('hsuser.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

       .state('hsuser.hs-code-operation', {
           url: '/hs-code-operation',
           templateUrl: 'hsCode/hsCodeOperation.tpl.html',
           controller: 'HSCodeController',
           data: { pageTitle: 'HSCode Operation' }
       })
              .state('hsuser.hscode-jobs-dashboard', {
                  url: '/hscode-jobs-dashboard',
                  templateUrl: 'jobs/jobsDashboard.tpl.html',
                  controller: 'JobsDashboradController',
                  data: { pageTitle: 'Jobs DasdhBoard' }
              })
        // HSCode Opeartor routing end


        // HSCode Manager Routing

         .state('hsmanager', {
             abstract: true,
             url: '/hsmanager',
             controller: 'LoginViewController',
             templateUrl: 'loginView/loginView.tpl.html'
         })

        //Direct Booking Routing
         .state('hsmanager.booking-home', {
             url: '/booking-home',
             abstract: true,
             templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
             //  controller: 'BookingHomeController',
             data: { pageTitle: 'Booking Home' }
         })
        .state('hsmanager.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('hsmanager.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
          .state('hsmanager.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
         .state('hsmanager.booking-home.direct-booking-return', {
             url: '/direct-booking-return/:directShipmentId',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
         })
        .state('hsmanager.booking-home.address-book', {
            url: '/address-book/:userId',
            templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
            controller: 'CustomerAddressBookController',
            data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
        })

          .state('hsmanager.hscode-jobs-dashboard', {
              url: '/hscode-jobs-dashboard',
              templateUrl: 'jobs/jobsDashboard.tpl.html',
              controller: 'JobsDashboradController',
              data: { pageTitle: 'Jobs DasdhBoard' }
          })

          .state('hsmanager.no-hscode-jobs', {
              url: '/no-hscode-jobs',
              templateUrl: 'jobs/jobsWithNoHSCodes/noHSCodeJobs.tpl.html',
              controller: 'NoHSCodeJobsController',
              data: { pageTitle: 'Jobs With No Hs Codes' }
          })

          .state('hsmanager.hscode-jobs', {
              url: '/hscode-jobs',
              templateUrl: 'jobs/hsCodeJobs/hsCodeJobs.tpl.html',
              controller: 'HSCodeJobsController',
              data: { pageTitle: 'Jobs With HSCodes' }
          })

          .state('hsmanager.hscode-jobs-perhour', {
              url: '/hscode-jobs-perhour',
              templateUrl: 'jobs/jobsPerHour/jobsPerHour.tpl.html',
              controller: 'JobsPerHourController',
              data: { pageTitle: 'Jobs Per Hour' }
          })


        .state('hsmanager.hscode-opeartors', {
            url: '/hscode-hscode-opeartors',
            templateUrl: 'jobs/hsCodeOperators/hsCodeOperator.tpl.html',
            controller: 'HSCodeOperatorController',
            data: { pageTitle: 'Booking Home' }
        })


             
         .state('hsmanager.manifests', {
             url: '/manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
         .state('hsmanager.manifests.user-manifests', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('hsmanager.manifests.custom-manifests', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
        // Track And Trace
         .state('hsmanager.direct-shipments', {
             url: '/direct-shipments',
             templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
             controller: 'DirectShipmentController',
             data: { pageTitle: 'Track & Trace' }
         })

        // Setting Routing
       .state('hsmanager.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('hsmanager.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('hsmanager.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('hsmanager.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('hsmanager.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('hsmanager.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('hsmanager.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('hsmanager.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('hsmanager.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('hsmanager.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })


             .state('hsmanager.zone-setting', {
                 abstract: true,
                 url: '/zone-setting',
                 templateUrl: 'zoneSetting/zoneSetting.tpl.html',
                 controller: 'ZoneSettingController',
                 data: { pageTitle: 'Zone Setting' }
             })
     .state('hsmanager.zone-setting.zone-country', {
         url: '/zone-country',
         templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
         controller: 'ZoneCountryController',
         data: { pageTitle: 'Country Zone' }
     })
      .state('hsmanager.zone-setting.third-party-matrix', {
          url: '/third-party-matrix',
          templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
          controller: 'ThirdPartyMatrixController',
          data: { pageTitle: '3rd Party Matrix' }
      })
      .state('hsmanager.zone-setting.base-rate-card', {
          url: '/base-rate-card',
          templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
          controller: 'BaseRateCardController',
          data: { pageTitle: ' Base Rate Card' }
      })
      .state('hsmanager.zone-setting.zone-postCode', {
          url: '/zone-postCode',
          templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
          controller: 'zonePostCodeController',
          data: { pageTitle: ' Zone Post/Zip Code' }
      })
        .state('hsmanager.zone-setting.countryzone-postCode', {
            url: '/countryzone-postCode',
            templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
            controller: 'CountryZonePostCodeController',
            data: { pageTitle: ' Country Zone Post/Zip Code' }
        })
       .state('hsmanager.quotation-tools', {
           url: '/quotation-tools',
           templateUrl: 'quotationTools/quotationTools.tpl.html',
           controller: 'QuotationToolController',
           data: { pageTitle: 'Quotation' }
       })
            .state('hsmanager.user', {
                url: '/user',
                templateUrl: 'user/user.tpl.html',
                controller: 'UserController',
                data: { pageTitle: 'Users' }
            })

        .state('hsmanager.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })
          .state('hsmanager.customers', {
              url: '/customers',
              templateUrl: 'customer/customer.tpl.html',
              controller: 'CustomerController',
              data: { pageTitle: 'Customers' }
          })

    .state('hsmanager.customer-detail', {
        abstract: true,
        url: '/customer-detail/:customerId',
        templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
        controller: 'CustomerBasicController',
        data: { pageTitle: 'Customer Detail' }
    })
      .state('hsmanager.customer-detail.basic-detail', {
          url: '/basic-detail',
          templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
          controller: 'CustomerBasicDetailController',
          data: { pageTitle: 'Customer Basic Detail' }
      })
      .state('hsmanager.customer-detail.margincost', {
          url: '/margincost',
          templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
          controller: 'CustomerMarginCostController',
          data: { pageTitle: 'Customer MarginCost' }
      })
         .state('hsmanager.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer AdvanceRateCard' }
         })
        .state('hsmanager.customer-detail.customerRateCard', {
            url: '/customerRateCard',
            templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
            controller: 'CustomerRateCardSettingController',
            data: { pageTitle: 'Customer RateCard' }
        })
                 .state('hsmanager.courier-accounts', {
                     url: '/courier-accounts',
                     abstract: true,
                     templateUrl: 'courierAccount/countryAccount.tpl.html',
                     controller: 'CourierAccountController',
                     data: { pageTitle: 'Couriers Account' }
                 })
           .state('hsmanager.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('hsmanager.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           // Access-Level
         .state('hsmanager.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

       .state('hsmanager.hs-code-operation', {
           url: '/hs-code-operation',
           templateUrl: 'hsCode/hsCodeOperation.tpl.html',
           controller: 'HSCodeController',
           data: { pageTitle: 'HSCode Operation' }
       })

        // HSCodeManager routing end



            // -- Agent State Start --

        .state('agent', {
            abstract: true,
            url: '/agent',
            controller: 'LoginViewController',
            templateUrl: 'loginView/loginView.tpl.html'
        })

          .state('agent.customers', {
              url: '/customers',
              templateUrl: 'customer/customer.tpl.html',
              controller: 'CustomerController',
              data: { pageTitle: 'Customers' }
          })

        .state('agent.customer-detail', {
            abstract: true,
            url: '/customer-detail/:customerId',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerBasicController',
            data: { pageTitle: 'Customer Detail' }
        })
          .state('agent.customer-detail.basic-detail', {
              url: '/basic-detail',
              templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
              controller: 'CustomerBasicDetailController',
              data: { pageTitle: 'Customer Basic Detail' }
          })
          .state('agent.customer-detail.margincost', {
              url: '/margincost',
              templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
              controller: 'CustomerMarginCostController',
              data: { pageTitle: 'Customer Margin Cost' }
          })
         .state('agent.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer Advance Rate Card' }
         })
            .state('agent.customer-detail.customerRateCard', {
                url: '/customerRateCard',
                templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
                controller: 'CustomerRateCardSettingController',
                data: { pageTitle: 'Customer Rate Card' }
            })
           .state('agent.courier-accounts', {
               url: '/courier-accounts',
               abstract: true,
               templateUrl: 'courierAccount/countryAccount.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('agent.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('agent.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
         .state('agent.zone-setting', {
             abstract: true,
             url: '/zone-setting',
             templateUrl: 'zoneSetting/zoneSetting.tpl.html',
             controller: 'ZoneSettingController',
             data: { pageTitle: 'Zone Setting' }
         })
         .state('agent.zone-setting.zone-country', {
             url: '/zone-country',
             templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
             controller: 'ZoneCountryController',
             data: { pageTitle: 'Country Zone' }
         })
          .state('agent.zone-setting.third-party-matrix', {
              url: '/third-party-matrix',
              templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
              controller: 'ThirdPartyMatrixController',
              data: { pageTitle: '3rd Party Matrix' }
          })
          .state('agent.zone-setting.base-rate-card', {
              url: '/base-rate-card',
              templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
              controller: 'BaseRateCardController',
              data: { pageTitle: ' Base Rate Card' }
          })
          .state('agent.zone-setting.zone-postCode', {
              url: '/zone-postCode',
              templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
              controller: 'zonePostCodeController',
              data: { pageTitle: ' Zone Post/Zip Code' }
          })
        .state('agent.zone-setting.countryzone-postCode', {
            url: '/countryzone-postCode',
            templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
            controller: 'CountryZonePostCodeController',
            data: { pageTitle: ' Country Zone Post/Zip Code' }
        })
                 .state('agent.booking-home.address-book', {
                     url: '/address-book/:userId',
                     templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
                     controller: 'CustomerAddressBookController',
                     data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
                 })
             
        .state('agent.manifests', {
            url: '/manifests',
            templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
            controller: 'CustomerManifestController',
            data: { pageTitle: 'Manifests' }
        })
         .state('agent.manifests.user-manifests', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('agent.manifests.custom-manifests', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
          .state('agent.user', {
              url: '/user',
              templateUrl: 'user/user.tpl.html',
              controller: 'UserController',
              data: { pageTitle: 'Users' }
          })

        .state('agent.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })

          //Direct Booking Routing
         .state('agent.booking-home', {
             url: '/booking-home',
             abstract: true,
             templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
             //  controller: 'BookingHomeController',
             data: { pageTitle: 'Booking Home' }
         })
        .state('agent.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('agent.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
          .state('agent.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
         .state('agent.booking-home.direct-booking-return', {
             url: '/direct-booking-return/:directShipmentId',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
         })

        // Track And Trace
         .state('agent.direct-shipments', {
             url: '/direct-shipments',
             templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
             controller: 'DirectShipmentController',
             data: { pageTitle: 'Track & Trace' }
         })

        // Setting Routing
       .state('agent.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('agent.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('agent.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('agent.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('agent.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('agent.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('agent.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('agent.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('agent.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('agent.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })

           // Access-Level
         .state('agent.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('agent.hs-code-operation', {
            url: '/hs-code-operation',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })
                .state('agent.hscode-jobs-dashboard', {
                    url: '/hscode-jobs-dashboard',
                    templateUrl: 'jobs/jobsDashboard.tpl.html',
                    controller: 'JobsDashboradController',
                    data: { pageTitle: 'Jobs DasdhBoard' }
                })
         .state('agent.quotation-tools', {
             url: '/quotation-tools',
             templateUrl: 'quotationTools/quotationTools.tpl.html',
             controller: 'QuotationToolController',
             data: { pageTitle: 'Quotation' }
         })

    // -- Agent State End --


    // -- Warehouse User State Start --

        .state('whuser', {
            abstract: true,
            url: '/whuser',
            controller: 'LoginViewController',
            templateUrl: 'loginView/loginView.tpl.html'
        })

          .state('whuser.customers', {
              url: '/customers',
              templateUrl: 'customer/customer.tpl.html',
              controller: 'CustomerController',
              data: { pageTitle: 'Customers' }
          })

        .state('whuser.customer-detail', {
            abstract: true,
            url: '/customer-detail/:customerId',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerBasicController',
            data: { pageTitle: 'Customer Detail' }
        })
          .state('whuser.customer-detail.basic-detail', {
              url: '/basic-detail',
              templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
              controller: 'CustomerBasicDetailController',
              data: { pageTitle: 'Customer Basic Detail' }
          })
          .state('whuser.customer-detail.margincost', {
              url: '/margincost',
              templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
              controller: 'CustomerMarginCostController',
              data: { pageTitle: 'Customer Margin Cost' }
          })
         .state('whuser.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer Advance Rate Card' }
         })
            .state('whuser.customer-detail.customerRateCard', {
                url: '/customerRateCard',
                templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
                controller: 'CustomerRateCardSettingController',
                data: { pageTitle: 'Customer Rate Card' }
            })
           .state('whuser.courier-accounts', {
               url: '/courier-accounts',
               abstract: true,
               templateUrl: 'courierAccount/countryAccount.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('whuser.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('whuser.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
         .state('whuser.zone-setting', {
             abstract: true,
             url: '/zone-setting',
             templateUrl: 'zoneSetting/zoneSetting.tpl.html',
             controller: 'ZoneSettingController',
             data: { pageTitle: 'Zone Setting' }
         })
         .state('whuser.zone-setting.zone-country', {
             url: '/zone-country',
             templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
             controller: 'ZoneCountryController',
             data: { pageTitle: 'Country Zone' }
         })
          .state('whuser.zone-setting.third-party-matrix', {
              url: '/third-party-matrix',
              templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
              controller: 'ThirdPartyMatrixController',
              data: { pageTitle: '3rd Party Matrix' }
          })
          .state('whuser.zone-setting.base-rate-card', {
              url: '/base-rate-card',
              templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
              controller: 'BaseRateCardController',
              data: { pageTitle: ' Base Rate Card' }
          })
          .state('whuser.zone-setting.zone-postCode', {
              url: '/zone-postCode',
              templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
              controller: 'zonePostCodeController',
              data: { pageTitle: ' Zone Post/Zip Code' }
          })
        .state('whuser.zone-setting.countryzone-postCode', {
            url: '/countryzone-postCode',
            templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
            controller: 'CountryZonePostCodeController',
            data: { pageTitle: ' Country Zone Post/Zip Code' }
        })
                 .state('whuser.booking-home.address-book', {
                     url: '/address-book/:userId',
                     templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
                     controller: 'CustomerAddressBookController',
                     data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
                 })
           
        .state('whuser.manifests', {
            url: '/manifests',
            templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
            controller: 'CustomerManifestController',
            data: { pageTitle: 'Manifests' }
        })
         .state('whuser.manifests.user-manifests', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('whuser.manifests.custom-manifests', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
          .state('whuser.user', {
              url: '/user',
              templateUrl: 'user/user.tpl.html',
              controller: 'UserController',
              data: { pageTitle: 'Users' }
          })

        .state('whuser.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })

          //Direct Booking Routing
         .state('whuser.booking-home', {
             url: '/booking-home',
             abstract: true,
             templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
             //  controller: 'BookingHomeController',
             data: { pageTitle: 'Booking Home' }
         })
        .state('whuser.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('whuser.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
          .state('whuser.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
         .state('whuser.booking-home.direct-booking-return', {
             url: '/direct-booking-return/:directShipmentId',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
         })

        // Track And Trace
         .state('whuser.direct-shipments', {
             url: '/direct-shipments',
             templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
             controller: 'DirectShipmentController',
             data: { pageTitle: 'Track & Trace' }
         })

        // Setting Routing
       .state('whuser.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('whuser.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('whuser.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('whuser.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('whuser.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('whuser.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('whuser.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('whuser.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('whuser.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('whuser.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })

           // Access-Level
         .state('whuser.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('whuser.hs-code-operation', {
            url: '/hs-code-operation',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })
                 .state('whuser.hscode-jobs-dashboard', {
                     url: '/hscode-jobs-dashboard',
                     templateUrl: 'jobs/jobsDashboard.tpl.html',
                     controller: 'JobsDashboradController',
                     data: { pageTitle: 'Jobs DasdhBoard' }
                 })
         .state('whuser.quotation-tools', {
             url: '/quotation-tools',
             templateUrl: 'quotationTools/quotationTools.tpl.html',
             controller: 'QuotationToolController',
             data: { pageTitle: 'Quotation' }
         })

    // -- Warehouse User State End --


      // -- Warehouse - Agent User State Start --

        .state('whagent', {
            abstract: true,
            url: '/whagent',
            controller: 'LoginViewController',
            templateUrl: 'loginView/loginView.tpl.html'
        })

          .state('whagent.customers', {
              url: '/customers',
              templateUrl: 'customer/customer.tpl.html',
              controller: 'CustomerController',
              data: { pageTitle: 'Customers' }
          })

        .state('whagent.customer-detail', {
            abstract: true,
            url: '/customer-detail/:customerId',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerBasicController',
            data: { pageTitle: 'Customer Detail' }
        })
          .state('whagent.customer-detail.basic-detail', {
              url: '/basic-detail',
              templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
              controller: 'CustomerBasicDetailController',
              data: { pageTitle: 'Customer Basic Detail' }
          })
          .state('whagent.customer-detail.margincost', {
              url: '/margincost',
              templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
              controller: 'CustomerMarginCostController',
              data: { pageTitle: 'Customer Margin Cost' }
          })
         .state('whagent.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer Advance Rate Card' }
         })
            .state('whagent.customer-detail.customerRateCard', {
                url: '/customerRateCard',
                templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
                controller: 'CustomerRateCardSettingController',
                data: { pageTitle: 'Customer Rate Card' }
            })
           .state('whagent.courier-accounts', {
               url: '/courier-accounts',
               abstract: true,
               templateUrl: 'courierAccount/countryAccount.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('whagent.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('whagent.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
         .state('whagent.zone-setting', {
             abstract: true,
             url: '/zone-setting',
             templateUrl: 'zoneSetting/zoneSetting.tpl.html',
             controller: 'ZoneSettingController',
             data: { pageTitle: 'Zone Setting' }
         })
         .state('whagent.zone-setting.zone-country', {
             url: '/zone-country',
             templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
             controller: 'ZoneCountryController',
             data: { pageTitle: 'Country Zone' }
         })
          .state('whagent.zone-setting.third-party-matrix', {
              url: '/third-party-matrix',
              templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
              controller: 'ThirdPartyMatrixController',
              data: { pageTitle: '3rd Party Matrix' }
          })
          .state('whagent.zone-setting.base-rate-card', {
              url: '/base-rate-card',
              templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
              controller: 'BaseRateCardController',
              data: { pageTitle: ' Base Rate Card' }
          })
          .state('whagent.zone-setting.zone-postCode', {
              url: '/zone-postCode',
              templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
              controller: 'zonePostCodeController',
              data: { pageTitle: ' Zone Post/Zip Code' }
          })
        .state('whagent.zone-setting.countryzone-postCode', {
            url: '/countryzone-postCode',
            templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
            controller: 'CountryZonePostCodeController',
            data: { pageTitle: ' Country Zone Post/Zip Code' }
        })
                 .state('whagent.booking-home.address-book', {
                     url: '/address-book/:userId',
                     templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
                     controller: 'CustomerAddressBookController',
                     data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
                 })
            
        .state('whagent.manifests', {
            url: '/manifests',
            templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
            controller: 'CustomerManifestController',
            data: { pageTitle: 'Manifests' }
        })
         .state('whagent.manifests.user-manifests', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('whagent.manifests.custom-manifests', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
          .state('whagent.user', {
              url: '/user',
              templateUrl: 'user/user.tpl.html',
              controller: 'UserController',
              data: { pageTitle: 'Users' }
          })

        .state('whagent.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })

          //Direct Booking Routing
         .state('whagent.booking-home', {
             url: '/booking-home',
             abstract: true,
             templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
             //  controller: 'BookingHomeController',
             data: { pageTitle: 'Booking Home' }
         })
        .state('whagent.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('whagent.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
          .state('whagent.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
         .state('whagent.booking-home.direct-booking-return', {
             url: '/direct-booking-return/:directShipmentId',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
         })

        // Track And Trace
         .state('whagent.direct-shipments', {
             url: '/direct-shipments',
             templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
             controller: 'DirectShipmentController',
             data: { pageTitle: 'Track & Trace' }
         })

        // Setting Routing
       .state('whagent.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('whagent.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('whagent.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('whagent.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('whagent.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('whagent.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('whagent.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('whagent.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('whagent.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('whagent.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })

           // Access-Level
         .state('whagent.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('whagent.hs-code-operation', {
            url: '/hs-code-operation',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })
                      .state('whagent.hscode-jobs-dashboard', {
                          url: '/hscode-jobs-dashboard',
                          templateUrl: 'jobs/jobsDashboard.tpl.html',
                          controller: 'JobsDashboradController',
                          data: { pageTitle: 'Jobs DasdhBoard' }
                      })
         .state('whagent.quotation-tools', {
             url: '/quotation-tools',
             templateUrl: 'quotationTools/quotationTools.tpl.html',
             controller: 'QuotationToolController',
             data: { pageTitle: 'Quotation' }
         })

    // -- Warehouse User State End --

     // -- Call Center USer State Start --

        .state('ccuser', {
            abstract: true,
            url: '/ccuser',
            controller: 'LoginViewController',
            templateUrl: 'loginView/loginView.tpl.html'
        })

          .state('ccuser.customers', {
              url: '/customers',
              templateUrl: 'customer/customer.tpl.html',
              controller: 'CustomerController',
              data: { pageTitle: 'Customers' }
          })

        .state('ccuser.customer-detail', {
            abstract: true,
            url: '/customer-detail/:customerId',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerBasicController',
            data: { pageTitle: 'Customer Detail' }
        })
          .state('ccuser.customer-detail.basic-detail', {
              url: '/basic-detail',
              templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
              controller: 'CustomerBasicDetailController',
              data: { pageTitle: 'Customer Basic Detail' }
          })
          .state('ccuser.customer-detail.margincost', {
              url: '/margincost',
              templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
              controller: 'CustomerMarginCostController',
              data: { pageTitle: 'Customer Margin Cost' }
          })
         .state('ccuser.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer Advance Rate Card' }
         })
            .state('ccuser.customer-detail.customerRateCard', {
                url: '/customerRateCard',
                templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
                controller: 'CustomerRateCardSettingController',
                data: { pageTitle: 'Customer Rate Card' }
            })
           .state('ccuser.courier-accounts', {
               url: '/courier-accounts',
               abstract: true,
               templateUrl: 'courierAccount/countryAccount.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('ccuser.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('ccuser.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
         .state('ccuser.zone-setting', {
             abstract: true,
             url: '/zone-setting',
             templateUrl: 'zoneSetting/zoneSetting.tpl.html',
             controller: 'ZoneSettingController',
             data: { pageTitle: 'Zone Setting' }
         })
         .state('ccuser.zone-setting.zone-country', {
             url: '/zone-country',
             templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
             controller: 'ZoneCountryController',
             data: { pageTitle: 'Country Zone' }
         })
          .state('ccuser.zone-setting.third-party-matrix', {
              url: '/third-party-matrix',
              templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
              controller: 'ThirdPartyMatrixController',
              data: { pageTitle: '3rd Party Matrix' }
          })
          .state('ccuser.zone-setting.base-rate-card', {
              url: '/base-rate-card',
              templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
              controller: 'BaseRateCardController',
              data: { pageTitle: ' Base Rate Card' }
          })
          .state('ccuser.zone-setting.zone-postCode', {
              url: '/zone-postCode',
              templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
              controller: 'zonePostCodeController',
              data: { pageTitle: ' Zone Post/Zip Code' }
          })
        .state('ccuser.zone-setting.countryzone-postCode', {
            url: '/countryzone-postCode',
            templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
            controller: 'CountryZonePostCodeController',
            data: { pageTitle: ' Country Zone Post/Zip Code' }
        })
                 .state('ccuser.booking-home.address-book', {
                     url: '/address-book/:userId',
                     templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
                     controller: 'CustomerAddressBookController',
                     data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
                 })
             
         .state('ccuser.manifests', {
             url: '/manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
         .state('ccuser.manifests.user-manifests', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('ccuser.manifests.custom-manifests', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
          .state('ccuser.user', {
              url: '/user',
              templateUrl: 'user/user.tpl.html',
              controller: 'UserController',
              data: { pageTitle: 'Users' }
          })

        .state('ccuser.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })

          //Direct Booking Routing
         .state('ccuser.booking-home', {
             url: '/booking-home',
             abstract: true,
             templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
             //  controller: 'BookingHomeController',
             data: { pageTitle: 'Booking Home' }
         })
        .state('ccuser.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('ccuser.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
          .state('ccuser.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
         .state('ccuser.booking-home.direct-booking-return', {
             url: '/direct-booking-return/:directShipmentId',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
         })

        // Track And Trace
         .state('ccuser.direct-shipments', {
             url: '/direct-shipments',
             templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
             controller: 'DirectShipmentController',
             data: { pageTitle: 'Track & Trace' }
         })

        // Setting Routing
       .state('ccuser.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('ccuser.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('ccuser.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('ccuser.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('ccuser.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('ccuser.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('ccuser.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('ccuser.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('ccuser.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('ccuser.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })

           // Access-Level
         .state('ccuser.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('ccuser.hs-code-operation', {
            url: '/hs-code-operation',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })
                   .state('ccuser.hscode-jobs-dashboard', {
                       url: '/hscode-jobs-dashboard',
                       templateUrl: 'jobs/jobsDashboard.tpl.html',
                       controller: 'JobsDashboradController',
                       data: { pageTitle: 'Jobs DasdhBoard' }
                   })
         .state('ccuser.quotation-tools', {
             url: '/quotation-tools',
             templateUrl: 'quotationTools/quotationTools.tpl.html',
             controller: 'QuotationToolController',
             data: { pageTitle: 'Quotation' }
         })

    // -- Call Center User State End --

         // -- Call Center Manger USer State Start --

        .state('ccmanager', {
            abstract: true,
            url: '/ccmanager',
            controller: 'LoginViewController',
            templateUrl: 'loginView/loginView.tpl.html'
        })

          .state('ccmanager.customers', {
              url: '/customers',
              templateUrl: 'customer/customer.tpl.html',
              controller: 'CustomerController',
              data: { pageTitle: 'Customers' }
          })

        .state('ccmanager.customer-detail', {
            abstract: true,
            url: '/customer-detail/:customerId',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerBasicController',
            data: { pageTitle: 'Customer Detail' }
        })
          .state('ccmanager.customer-detail.basic-detail', {
              url: '/basic-detail',
              templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
              controller: 'CustomerBasicDetailController',
              data: { pageTitle: 'Customer Basic Detail' }
          })
          .state('ccmanager.customer-detail.margincost', {
              url: '/margincost',
              templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
              controller: 'CustomerMarginCostController',
              data: { pageTitle: 'Customer Margin Cost' }
          })
         .state('ccmanager.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer Advance Rate Card' }
         })
            .state('ccmanager.customer-detail.customerRateCard', {
                url: '/customerRateCard',
                templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
                controller: 'CustomerRateCardSettingController',
                data: { pageTitle: 'Customer Rate Card' }
            })
           .state('ccmanager.courier-accounts', {
               url: '/courier-accounts',
               abstract: true,
               templateUrl: 'courierAccount/countryAccount.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('ccmanager.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('ccmanager.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
         .state('ccmanager.zone-setting', {
             abstract: true,
             url: '/zone-setting',
             templateUrl: 'zoneSetting/zoneSetting.tpl.html',
             controller: 'ZoneSettingController',
             data: { pageTitle: 'Zone Setting' }
         })
         .state('ccmanager.zone-setting.zone-country', {
             url: '/zone-country',
             templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
             controller: 'ZoneCountryController',
             data: { pageTitle: 'Country Zone' }
         })
          .state('ccmanager.zone-setting.third-party-matrix', {
              url: '/third-party-matrix',
              templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
              controller: 'ThirdPartyMatrixController',
              data: { pageTitle: '3rd Party Matrix' }
          })
          .state('ccmanager.zone-setting.base-rate-card', {
              url: '/base-rate-card',
              templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
              controller: 'BaseRateCardController',
              data: { pageTitle: ' Base Rate Card' }
          })
          .state('ccmanager.zone-setting.zone-postCode', {
              url: '/zone-postCode',
              templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
              controller: 'zonePostCodeController',
              data: { pageTitle: ' Zone Post/Zip Code' }
          })
        .state('ccmanager.zone-setting.countryzone-postCode', {
            url: '/countryzone-postCode',
            templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
            controller: 'CountryZonePostCodeController',
            data: { pageTitle: ' Country Zone Post/Zip Code' }
        })
                 .state('ccmanager.booking-home.address-book', {
                     url: '/address-book/:userId',
                     templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
                     controller: 'CustomerAddressBookController',
                     data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
                 })
           
         .state('ccmanager.manifests', {
             url: '/manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
         .state('ccmanager.manifests.user-manifests', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('ccmanager.manifests.custom-manifests', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
          .state('ccmanager.user', {
              url: '/user',
              templateUrl: 'user/user.tpl.html',
              controller: 'UserController',
              data: { pageTitle: 'Users' }
          })

        .state('ccmanager.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })

          //Direct Booking Routing
         .state('ccmanager.booking-home', {
             url: '/booking-home',
             abstract: true,
             templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
             //  controller: 'BookingHomeController',
             data: { pageTitle: 'Booking Home' }
         })
        .state('ccmanager.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('ccmanager.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
          .state('ccmanager.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
         .state('ccmanager.booking-home.direct-booking-return', {
             url: '/direct-booking-return/:directShipmentId',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
         })

        // Track And Trace
         .state('ccmanager.direct-shipments', {
             url: '/direct-shipments',
             templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
             controller: 'DirectShipmentController',
             data: { pageTitle: 'Track & Trace' }
         })

        // Setting Routing
       .state('ccmanager.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('ccmanager.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('ccmanager.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('ccmanager.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('ccmanager.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('ccmanager.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('ccmanager.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('ccmanager.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('ccmanager.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('ccmanager.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })

           // Access-Level
         .state('ccmanager.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('ccmanager.hs-code-operation', {
            url: '/hs-code-operation',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })
                 .state('ccmanager.hscode-jobs-dashboard', {
                     url: '/hscode-jobs-dashboard',
                     templateUrl: 'jobs/jobsDashboard.tpl.html',
                     controller: 'JobsDashboradController',
                     data: { pageTitle: 'Jobs DasdhBoard' }
                 })

         .state('ccmanager.quotation-tools', {
             url: '/quotation-tools',
             templateUrl: 'quotationTools/quotationTools.tpl.html',
             controller: 'QuotationToolController',
             data: { pageTitle: 'Quotation' }
         })

    // -- Call Center Manager State End --



             // -- Account USer State Start --

        .state('acuser', {
            abstract: true,
            url: '/acuser',
            controller: 'LoginViewController',
            templateUrl: 'loginView/loginView.tpl.html'
        })

          .state('acuser.customers', {
              url: '/customers',
              templateUrl: 'customer/customer.tpl.html',
              controller: 'CustomerController',
              data: { pageTitle: 'Customers' }
          })

        .state('acuser.customer-detail', {
            abstract: true,
            url: '/customer-detail/:customerId',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerBasicController',
            data: { pageTitle: 'Customer Detail' }
        })
          .state('acuser.customer-detail.basic-detail', {
              url: '/basic-detail',
              templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
              controller: 'CustomerBasicDetailController',
              data: { pageTitle: 'Customer Basic Detail' }
          })
          .state('acuser.customer-detail.margincost', {
              url: '/margincost',
              templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
              controller: 'CustomerMarginCostController',
              data: { pageTitle: 'Customer Margin Cost' }
          })
         .state('acuser.customer-detail.advanceratecard', {
             url: '/advanceratecard',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer Advance Rate Card' }
         })
            .state('acuser.customer-detail.customerRateCard', {
                url: '/customerRateCard',
                templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
                controller: 'CustomerRateCardSettingController',
                data: { pageTitle: 'Customer Rate Card' }
            })
           .state('acuser.courier-accounts', {
               url: '/courier-accounts',
               abstract: true,
               templateUrl: 'courierAccount/countryAccount.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('acuser.courier-accounts.easypost', {
               url: '/easypost-courier-accounts',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('acuser.courier-accounts.parcel-hub', {
               url: '/parcelhub-courier-accounts',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               //    controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
         .state('acuser.zone-setting', {
             abstract: true,
             url: '/zone-setting',
             templateUrl: 'zoneSetting/zoneSetting.tpl.html',
             controller: 'ZoneSettingController',
             data: { pageTitle: 'Zone Setting' }
         })
         .state('acuser.zone-setting.zone-country', {
             url: '/zone-country',
             templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
             controller: 'ZoneCountryController',
             data: { pageTitle: 'Country Zone' }
         })
          .state('acuser.zone-setting.third-party-matrix', {
              url: '/third-party-matrix',
              templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
              controller: 'ThirdPartyMatrixController',
              data: { pageTitle: '3rd Party Matrix' }
          })
          .state('acuser.zone-setting.base-rate-card', {
              url: '/base-rate-card',
              templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
              controller: 'BaseRateCardController',
              data: { pageTitle: ' Base Rate Card' }
          })
          .state('acuser.zone-setting.zone-postCode', {
              url: '/zone-postCode',
              templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
              controller: 'zonePostCodeController',
              data: { pageTitle: ' Zone Post/Zip Code' }
          })
        .state('acuser.zone-setting.countryzone-postCode', {
            url: '/countryzone-postCode',
            templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
            controller: 'CountryZonePostCodeController',
            data: { pageTitle: ' Country Zone Post/Zip Code' }
        })
                 .state('acuser.booking-home.address-book', {
                     url: '/address-book/:userId',
                     templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
                     controller: 'CustomerAddressBookController',
                     data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
                 })
          
        .state('acuser.manifests', {
            url: '/manifests',
            templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
            controller: 'CustomerManifestController',
            data: { pageTitle: 'Manifests' }
        })
         .state('acuser.manifests.user-manifests', {
             url: '/user-manifests',
             templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             controller: 'CustomerManifestController',
             data: { pageTitle: 'Manifests' }
         })
          .state('acuser.manifests.custom-manifests', {
              url: '/custom-manifests',
              templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
              controller: 'CustomerManifestController',
              data: { pageTitle: 'Manifests' }
          })
          .state('acuser.user', {
              url: '/user',
              templateUrl: 'user/user.tpl.html',
              controller: 'UserController',
              data: { pageTitle: 'Users' }
          })

        .state('acuser.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })

          //Direct Booking Routing
         .state('acuser.booking-home', {
             url: '/booking-home',
             abstract: true,
             templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
             //  controller: 'BookingHomeController',
             data: { pageTitle: 'Booking Home' }
         })
        .state('acuser.booking-home.booking-welcome', {
            url: '/booking-welcome',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
         .state('acuser.booking-home.direct-booking', {
             url: '/direct-booking/:directShipmentId/:callingtype',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
         })
          .state('acuser.booking-home.direct-booking-clone', {
              url: '/direct-booking-clone/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking Clone', IsFired: true, IsChanged: false }
          })
         .state('acuser.booking-home.direct-booking-return', {
             url: '/direct-booking-return/:directShipmentId',
             templateUrl: 'directBooking/directBooking.tpl.html',
             controller: 'DirectBookingController',
             data: { pageTitle: 'Direct Booking Return', IsFired: true, IsChanged: false }
         })

        // Track And Trace
         .state('acuser.direct-shipments', {
             url: '/direct-shipments',
             templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
             controller: 'DirectShipmentController',
             data: { pageTitle: 'Track & Trace' }
         })

        // Setting Routing
       .state('acuser.setting', {
           abstract: true,
           url: '/setting',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('acuser.setting.margin-rate', {
            url: '/margin-rate',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('acuser.setting.systemAlerts', {
             url: '/system-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('acuser.setting.terms-and-condition', {
            url: '/terms-and-condition',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('acuser.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('acuser.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('acuser.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('acuser.setting.parcel-hub', {
            url: '/parcel-hub',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('acuser.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('acuser.setting.fuelSurCharge', {
            url: '/fuelSurCharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })

           // Access-Level
         .state('acuser.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('acuser.hs-code-operation', {
            url: '/hs-code-operation',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })
                     .state('acuser.hscode-jobs-dashboard', {
                         url: '/hscode-jobs-dashboard',
                         templateUrl: 'jobs/jobsDashboard.tpl.html',
                         controller: 'JobsDashboradController',
                         data: { pageTitle: 'Jobs DasdhBoard' }
                     })

         .state('acuser.quotation-tools', {
             url: '/quotation-tools',
             templateUrl: 'quotationTools/quotationTools.tpl.html',
             controller: 'QuotationToolController',
             data: { pageTitle: 'Quotation' }
         })

    // -- Account User State End --
    ;
})
;