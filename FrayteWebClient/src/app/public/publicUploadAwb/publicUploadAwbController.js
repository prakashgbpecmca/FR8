/** 
 * Controller
 */
angular.module('ngApp.public').controller('UploadAWBController', function ($scope, $state, $stateParams, $translate, $location, config, $filter, SessionService, ShipmentService, Upload, $timeout, toaster) {

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

    $scope.isLogin = false;

    $scope.uploadDocument = {
        AirWayBillHash: '',
        AirWayBillDocumentFileName: '',
        AirWayBillDocument: ''
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
            $scope.uploadDocument.AirWayBillDocumentFileName = $file.name;
            $scope.uploadDocument.AirWayBillDocument = $file.name;
        }
    };

    $scope.upload = function (isValid, AirWayBillDocumentFileName, exPortType) {
        if ($scope.uploadDocument.AirWayBillDocumentFileName === '' && exPortType==='i') {
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
            if (AirWayBillDocumentFileName !== undefined && AirWayBillDocumentFileName !== null) {
                files.push(AirWayBillDocumentFileName);
            }
            $scope.upload = Upload.upload({
                url: config.SERVICE_URL + '/Shipment/UploadAirWayBill',
                fields: {
                    ShipmentType: $scope.ShipmentType,
                    ShipmentId: $scope.shipmentId,
                    AirWayBillHash: $scope.uploadDocument.AirWayBillHash,
                    AirWayBillDocumentFileName: $scope.uploadDocument.AirWayBillDocumentFileName,
                    AirWayBillDocument: $scope.uploadDocument.AirWayBillDocument
                },
                file: files
            });

            $scope.upload.progress($scope.progress);
            $scope.upload.success($scope.success);
            $scope.upload.error($scope.error);

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
        if (status = 200) {
            toaster.pop({
                type: 'success',
                title: $scope.TitleFrayteSuccess,
                body: $scope.TextUploadedSuccessfully,
                showCloseButton: true
            });

            if ($stateParams.shipmentId === undefined || $stateParams.shipmentId === null) {
                $scope.$dismiss('cancel');
            }
            else {
                //Redirect to mail page after 4 second.
                $timeout(function () {
                    $state.go('home.welcome');
                }, 4000);
            }
        }
    };

    $scope.error = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scope.TextErrorOccuredDuringUpload,
            showCloseButton: true
        });
    };
    $scope.shipmentId = '';
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

        var userInfo = SessionService.getUser();
        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            $scope.isLogin = false;
        }
        else {
            $scope.isLogin = true;
        }

        if ($stateParams.shipmentId === undefined || $stateParams.shipmentId === null) {
            $scope.shipmentId = $scope.params.shipmentId;
        }
        else {
            $scope.ShipmentType = $stateParams.exportType;
            if ($scope.ShipmentType === 'e')
            {
               $scope.exPortType = false;
            }
            if ($scope.ShipmentType === 'i') {
                $scope.exPortType = true;
            }
            $scope.shipmentId = $stateParams.shipmentId;
            var id = $stateParams.shipmentId;
            if (!$scope.isLogin) {
                getShipmentDetail(id);
            }
        }
    }
    init();

});