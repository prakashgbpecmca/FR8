/** 
 * Controller
 */
angular.module('ngApp.public').controller('AgentUploadAWBShipmentController', function ($scope, $state, $translate, $stateParams, $location, config, $filter, SessionService, ShipmentService, Upload, $timeout, toaster) {

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

    var setUIText = function (shipmentType) {
        $translate(['AgentUploadMasterAWBDocument', 'AgentUploadBillOfLadingDocument', 'In', 'AWB', 'Master', 'Bill', 'of', 'Lading']).then(function (translations) {
            if (shipmentType === "s") {
                $scope.AWBHeading = translations.AgentUploadBillOfLadingDocument;
                $scope.InAWB = translations.Master + ' ' + translations.Bill + ' ' + translations.of + ' ' + translations.Lading;
            }
            else if (shipmentType === "a") {
                $scope.AWBHeading = translations.AgentUploadMasterAWBDocument;
                $scope.InAWB = translations.In + ' ' + translations.AWB;
            }
           
        });
    };
    $scope.publicAwbDocument = {
        ShipmentId: 0,
        AWBFileName: '',
        AWB: '',
        OtherDocsFileName: '',
        OtherDocs: ''
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
            $scope.publicAwbDocument.AWBFileName = $file.name;
            $scope.publicAwbDocument.AWB = $file.name;
        }
        else if (type === 2) {
            $scope.publicAwbDocument.OtherDocsFileName = $file.name;
            $scope.publicAwbDocument.OtherDocs = $file.name;
        }
    };


    $scope.AgentUploadAWBDocs = function (isValid, AWBFileName, OtherDocsFileName) {
        if ($scope.publicAwbDocument.AWBFileName === '' &&
             $scope.publicAwbDocument.OtherDocsFileName === '') {
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
            if (AWBFileName !== undefined && AWBFileName !== null) {
                files.push(AWBFileName);
            }
            if (OtherDocsFileName !== undefined && OtherDocsFileName !== null) {
                files.push(OtherDocsFileName);
            }
            $scope.AgentUploadAWBDocs = Upload.upload({
                url: config.SERVICE_URL + '/Shipment/UploadAWBDetail',
                fields: {
                    ShipmentId: $scope.publicAwbDocument.ShipmentId,
                    AWBFileName: $scope.publicAwbDocument.AWBFileName,
                    AWB: $scope.publicAwbDocument.AWB,
                    OtherDocsFileName: $scope.publicAwbDocument.OtherDocsFileName,
                    OtherDocs: $scope.publicAwbDocument.OtherDocs
                },
                file: files
            });

            $scope.AgentUploadAWBDocs.progress($scope.progress);

            $scope.AgentUploadAWBDocs.success($scope.success);

            $scope.AgentUploadAWBDocs.error($scope.error);
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


    $scope.confirmationCode = '';
    //$scope.shipmentId = '';
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

        $scope.publicAwbDocument.ShipmentId = $stateParams.shipmentId;
        var id = $stateParams.shipmentId;
        var shipementType = $stateParams.shipmentType;
        getShipmentDetail(id);
        setUIText(shipementType);
    }

    init();

});