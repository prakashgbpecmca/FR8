/** 
 * Controller
 */
angular.module('ngApp.public').controller('AgentUploadPodShipmentController', function ($scope, $state, $translate, $stateParams, $location, config, $filter, SessionService, ShipmentService, Upload, $timeout, toaster) {

    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'FrayteError', 'FrayteValidation', 'ErrorGetting', 'PleaseCorrectValidationErrors', 'PleaseSelectValidFile', 'ErrorOccuredDuringUpload', 'UploadedSuccessfully', 'PleaseAttachDocument', 'agent', 'records']).then(function (translations) {

            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextPleaseSelectValidFile = translations.PleaseSelectValidFile;
            $scope.TextPleaseAttachDocument = translations.PleaseAttachDocument;
            $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
            $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
            $scope.TextErrorGettingAgentRecord = translations.ErrorGetting + " " + translations.agent + " " + translations.records;

        });
    };

    $scope.agentPodUploadDocs = {
        ShipmentId: 0,
        SignatureImageFileName: '',
        SignatureImage: '',
        PODDeliveryDate:  new Date(),
        PODDeliveryTime: '',
        Signature: '',
        ExceptionNote: null
    };

    $scope.whileAddingDoc = function ($files, $file, $event, type) {

        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        if (type === 1) {
            $scope.agentPodUploadDocs.SignatureImageFileName = $file.name;
            $scope.agentPodUploadDocs.SignatureImage = $file.name;
        }
    };

    $scope.AgentPodDocs = function (isValid, SignatureImageFileName) {
        if ($scope.agentPodUploadDocs.SignatureImageFileName === '') {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseAttachDocument,
                showCloseButton: true
            });
            return;
        }

        if (isValid) {
            var files = [];
            if (SignatureImageFileName !== undefined && SignatureImageFileName !== null) {
                files.push(SignatureImageFileName);
            }
            $scope.AgentPodDocs = Upload.upload({
                url: config.SERVICE_URL + '/Shipment/UploadPODDetail',
                fields: {
                    ShipmentId: $scope.agentPodUploadDocs.ShipmentId,
                    SignatureImageFileName: $scope.agentPodUploadDocs.SignatureImageFileName,
                    SignatureImage: $scope.agentPodUploadDocs.SignatureImage,
                    PODDeliveryDate: $scope.agentPodUploadDocs.PODDeliveryDate,
                    PODDeliveryTime: $scope.agentPodUploadDocs.PODDeliveryTime,
                    Signature: $scope.agentPodUploadDocs.Signature,
                    ExceptionNote:$scope.agentPodUploadDocs.ExceptionNote
                },
                file: files
            });

            $scope.AgentPodDocs.progress($scope.progress);

            $scope.AgentPodDocs.success($scope.success);

            $scope.AgentPodDocs.error($scope.error);
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }

    };

    $scope.progress = function (evt) {
        //console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
        //toaster.clear();
        //toaster.pop({
        //    type: 'success',
        //    title: 'uploading',
        //    body: 'percent: ' + parseInt(100.0 * evt.loaded / evt.total, 10),
        //    showCloseButton: true
        //});
    };

    $scope.success = function (data, status, headers, config) {
        toaster.pop({
            type: 'success',
            title: $scope.TitleFrayteSuccess,
            body: $scope.TextUploadedSuccessfully,
            showCloseButton: true
        });

        $timeout(function () {
            $state.go('home.welcome');
        }, 4000);
    };

    $scope.error = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scope.TextErrorOccuredDuringUpload,
            showCloseButton: true
        });
    };

    $scope.openCalender = function ($event) {
        $scope.status.opened = true;
    };
    $scope.status = {
        opened: false
    };
    var getShipmentDetail = function (id) {
        ShipmentService.GetShipmentShipperReceiverDetail(id).then(function (response) {
            $scope.FrayteCargoWiseSo = response.data.FrayteCargoWiseSo;
            $scope.ShipperInfo = response.data.Shipper;
            $scope.ShipperAddressInfo = response.data.ShipperAddress;
            $scope.ReceiverInfo = response.data.Receiver;
            $scope.ReceiverAddressInfo = response.data.ReceiverAddress;

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingAgentRecord,
                showCloseButton: true
            });
        });
    };
    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.agentPodUploadDocs.ShipmentId = $stateParams.shipmentId;
        var id = $stateParams.shipmentId;
        getShipmentDetail(id);
    }
    init();
});