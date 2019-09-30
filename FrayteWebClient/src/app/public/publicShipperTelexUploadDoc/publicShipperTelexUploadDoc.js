/** 
 * Controller
 */
angular.module('ngApp.public').controller('PublicShipperTelexUploadController', function ($scope, $state, $stateParams, $translate, $location, config, $filter, SessionService, ShipmentService, Upload, $timeout, toaster) {

    var setModalOptions = function () {        
        $translate(['FrayteSuccess', 'FrayteError', 'FrayteValidation', 'ErrorGetting', 'PleaseCorrectValidationErrors', 'PleaseSelectValidFile', 'ErrorOccuredDuringUpload', 'UploadedSuccessfully', 'PleaseAttachDocument', 'agent', 'records']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TitleFrayteSuccess = translations.FrayteSuccess;

            $scope.TextErrorGettingAgentRecord = translations.ErrorGetting + " " + translations.agent + " " + translations.records;
            $scope.TextPleaseSelectValidFile = translations.PleaseSelectValidFile;
            $scope.TextPleaseAttachDocument = translations.PleaseAttachDocument;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
            $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
        });
    };

    var getShipmentShipperReceiverDetail = function (id) {
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

    // Shipment Initail detail 
    var getSeaInitial = function (shipmentId) {

        ShipmentService.getSeaInitial(shipmentId).then(function (response) {

            $scope.SeaInitialTelex = response.data;
        }, function () {
            console.log("error!");
        });
    };

    // while adding doc

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
            $scope.SeaInitialTelex.SeaTelexDocument = $file.name;
            $scope.SeaInitialTelex.SeaTelexDocumentName = $file.name;

        }
        if (type === 2) {
            $scope.SeaInitialTelex.SeaTelexStamp = $file.name;
            $scope.SeaInitialTelex.SeaTelexStampName = $file.name;
        }
    };

    // upload Telex Data
    $scope.uploadTelex = function (isValid, SeaInitialTelex, SeaTelexDocumentFile, SeaTelexStampFile) {
        if ($scope.SeaInitialTelex.SeaTelexDocument === '' &&
           $scope.SeaInitialTelex.SeaTelexStamp === '') {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseAttachDocument,
                showCloseButton: true
            });
            return;
        }
       
        if (isValid) {
            if (!$scope.confirmTelex) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: 'Please Confirm Telex Release.',
                    showCloseButton: true
                });
                return;
            }
            var files = [];
            if (SeaTelexDocumentFile !== undefined && SeaTelexDocumentFile !== null) {
                files.push(SeaTelexDocumentFile);
            }
            if (SeaTelexStampFile !== undefined && SeaTelexStampFile !== null) {
                files.push(SeaTelexStampFile);
            }

            // uplpad documents to server 
            $scope.upload = Upload.upload({
                url: config.SERVICE_URL + '/Shipment/UploadTelexDocuments',
                fields: {
                    ShipmentId: $scope.SeaInitialTelex.ShipmentId,
                    SeaPOL: $scope.SeaInitialTelex.SeaPOL,
                    SeaPOD: $scope.SeaInitialTelex.SeaPOD,
                    SeaTelexStampName: $scope.SeaInitialTelex.SeaTelexStampName,
                    TelexReleaseBy: $scope.SeaInitialTelex.TelexReleaseBy,
                    SeaTelexDocumentName: $scope.SeaInitialTelex.SeaTelexDocumentName,
                    SeaTelexDocument: $scope.SeaInitialTelex.SeaTelexDocument
                },
                file: files
            });

            //    ShipmentService.UpdatePOlPODDetail(SeaInitialTelex).then(function (response) {
            //        if (response.status) {
            //            $state.go('home.welcome');
            //        }
            //    });
            //}
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
        if (status == 200) {
            toaster.pop({
                type: 'success',
                title: $scope.TitleFrayteSuccess,
                body: $scope.TextUploadedSuccessfully,
                showCloseButton: true
            });

            if ($stateParams.shipmentId === undefined || $stateParams.shipmentId === null) {
                //  $scope.$dismiss('cancel');
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

    function init() {
        $scope.confirmTelex = false;
        //Translate 
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
            $scope.shipmentId = $stateParams.shipmentId;
            var id = $stateParams.shipmentId;
            if (!$scope.isLogin) {
                getShipmentShipperReceiverDetail(id);
                getSeaInitial(id);
            }
        }
    }
    init();

});