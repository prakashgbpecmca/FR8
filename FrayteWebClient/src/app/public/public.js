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
angular.module('ngApp.public', [
  'ui.router',
  'ngApp.common',
  'ngFileUpload'
])

/**
 * Each section or module of the site can also have its own routes. AngularJS
 * will handle ensuring they are all available at run-time, but splitting it
 * this way makes each module more "self-contained".
 */
.config(function config($stateProvider, $locationProvider) {
    //$locationProvider.html5Mode(true);
    $stateProvider
        .state('public', {
            abstract: true,
            url: '/public',
            templateUrl: 'public/publicHome.tpl.html',
            controller: 'PublicController'
        })
       .state('public.directBookingConfirmReject', {
                url: '/directBooking/:actionType/:directShipmentId',
                templateUrl: 'public/publicDirectBooking/directBookingConfirmReject.tpl.html',
                controller: 'DirectBookingConfirmRejectController',
                data: { pageTitle: 'Direct Booking' }
            })
        .state('public.shipmentdocument', {
            url: '/shipmentdocument/:shipmentId',
            templateUrl: 'public/shipmentDocument/shipmentDocument.tpl.html',
            controller: 'ShipmentDocumentController',
            data: { pageTitle: 'Upload Shipment Documents' }
        })

        .state('public.customer-action', {
            url: '/customer-action/:actionType/:confirmationCode',
            templateUrl: 'public/publicCustomerConfirmShipment/customerConfirmShipment.tpl.html',
            controller: 'PublicConfirmController',
            data: { pageTitle: 'Customer Action' }
        })
        .state('public.customer-ammend', {
            url: '/customer-amend/:userRoleId/:shipmentId',
            templateUrl: 'public/publicCustomerAmendShipment/customerAmendShipment.tpl.html',
            controller: 'PublicAmendController',
            data: { pageTitle: 'Customer Action' }
        })

        .state('public.agent-action', {
            url: '/agent-action/:actionType/:shipmentId',
            templateUrl: 'public/publicAgentActionShipment/agentActionShipment.tpl.html',
            controller: 'AgentActionController',
            data: { pageTitle: 'Agent Action' }
        })

        .state('public.warehouse-drop-off', {
            url: '/warehouse-drop-off/:shipmentId',
            templateUrl: 'public/publicShipmentDropOffTime/publicShipmentDropOffTime.tpl.html',
            controller: 'ShipmentDropOffTimeController',
            data: { pageTitle: 'Update Drop-Off Detail' }
        })

        .state('public.upload-awb', {
            url: '/upload-awb/:exportType/:shipmentId',
            templateUrl: 'public/publicUploadAwb/uploadAwb.tpl.html',
            controller: 'UploadAWBController',
            data: { pageTitle: 'Upload AWB Detail' }
        })

        .state('public.agent-anticipated', {
            url: '/agent-anticipated/:shipmentId',
            templateUrl: 'public/publicAgentAnticipatedShipment/publicAgentAnticipatedShipment.tpl.html',
            controller: 'AgentAnticipatedShipmentController',
            data: { pageTitle: 'Update Anticipated Detail' }
        })
        .state('public.shipper-anticipated', {
            url: '/shipper-anticipated/:shipmentId',
            templateUrl: 'public/publicAgentAnticipatedShipment/publicAgentAnticipatedShipment.tpl.html',
            controller: 'AgentAnticipatedShipmentController',
            data: { pageTitle: 'Update Anticipated Detail' }
        })
        .state('public.agent-flight-sea-document', {
            url: '/agent-flight-sea-document/:shipmentType/:shipmentId',
            templateUrl: 'public/publicAgentFlightSeaShipmentDocument/publicAgentFlightSeaShipmentDocument.tpl.html',
            controller: 'AgentFlightSeaShipmentDocumentController',
            data: { pageTitle: 'Update Detail' }
        })

        .state('public.agent-upload-awb', {
            url: '/agent-upload-awb/:shipmentType/:shipmentId',
            templateUrl: 'public/publicAgentUploadAWBShipment/publicAgentUploadAWBShipment.tpl.html',
            controller: 'AgentUploadAWBShipmentController',
            data: { pageTitle: 'Upload AWB Detail' }
        })
         .state('public.shipper-upload-awb', {
             url: '/shipper-upload-awb/:shipmentId',
             templateUrl: 'public/publicAgentUploadAWBShipment/publicAgentUploadAWBShipment.tpl.html',
             controller: 'AgentUploadAWBShipmentController',
             data: { pageTitle: 'Upload AWB Detail' }
         })

        .state('public.agent-reselect', {
            url: '/agent-reselect/:shipmentId',
            templateUrl: 'public/publicAgentReselectShipment/publicAgentReselectedShipment.tpl.html',
            controller: 'AgentReselectedShipmentController',
            data: { pageTitle: 'Agent Reselect' }
        })

        .state('public.agent-upload-pod', {
            url: '/agent-upload-pod/:shipmentId',
            templateUrl: 'public/publicAgentUploadPodShipment/publicAgentUploadPodShipment.tpl.html',
            controller: 'AgentUploadPodShipmentController',
            data: { pageTitle: 'Upload POD' }
        })

        .state('public.shipper-telex', {
            url: '/shipper-telex/:shipmentId',
            templateUrl: 'public/publicShipperTelexUploadDoc/publicShipperTelexUploadDoc.tpl.html',
            controller: 'PublicShipperTelexUploadController',
            data: { pageTitle: 'Shipper Telex' }
        })

    .state('public.operation-staff', {
        url: '/operation-staff/:shipmentId',
        templateUrl: 'public/publicOperationStaffConfirmShipment/operationStaffConfirm.tpl.html',
        controller: 'OperationStaffConfirmShipment',
        data: { pageTitle: 'Operation Staff' }
    })

     .state('public.admin-action', {
         url: '/admin-action/:customerId/:actionType/:operationUserId',
         templateUrl: 'public/publicAdminAction/adminConfirm.tpl.html',
         controller: 'AdminActionController',
         data: { pageTitle: 'Admin Confirmation' }
     })
    .state('public.term-and-condition', {
        url: '/term-and-condition',
        templateUrl: 'public/publicTermAndCondition/publicTermAndCondition.tpl.html',
        controller: 'PublicTermAndConditionController',
        data: { pageTitle: 'Terms And Condition' }
    })
    .state('public.trackingFAQ', {
        url: '/tracking-faqs',
        templateUrl: 'public/publicTrackingFAQs/trackingFAQs.tpl.html',
        controller: 'PublicTrackingFAQController',
        data: { pageTitle: 'Tracking FAQ' }
    })
    .state('public.systemAlertDetail', {
        url: '/systemAlertDetail/:Heading',
        templateUrl: 'public/publicSystemAlert/systemAlertDescription.tpl.html',
        controller: 'SystemAlertDescriptionController',
        data: { pageTitle: 'System Alert Description' }
    });
});


