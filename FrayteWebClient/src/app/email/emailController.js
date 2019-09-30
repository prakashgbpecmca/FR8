/** 
 * Controller
 */
angular.module('ngApp.email').controller('EmailController', function ($scope, $state, $location, $translate, config, $filter, SessionService, Upload, $timeout, toaster) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'Frayte-Error', 'FrayteValidation', 'PleaseSelectValidFile', 'ErrorOccuredDuringUpload', 'UploadedSuccessfully', 'PleaseAttachDocument']).then(function (translations) {

            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextPleaseSelectValidFile = translations.PleaseSelectValidFile;
            $scope.TextPleaseAttachDocument = translations.PleaseAttachDocument;
            $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
            $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;

        });
    };

    $scope.uploadDocument = {
        CommercialInvoiceFileName: '',
        CommercialInvoice: '',
        PackingListFileName: '',
        PackingList: '',
        CustomDocFileName: '',
        CustomDoc: ''
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
            $scope.uploadDocument.CommercialInvoiceFileName = $file.name;
            $scope.uploadDocument.CommercialInvoice = $file.name;
        }
        else if (type === 2) {
            $scope.uploadDocument.PackingListFileName = $file.name;
            $scope.uploadDocument.PackingList = $file.name;
        }
        else if (type === 3) {
            $scope.uploadDocument.CustomDocFileName = $file.name;
            $scope.uploadDocument.CustomDoc = $file.name;
        }
    };

    $scope.upload = function (isValid, commercialInvoiceFile, packingListFile, customDocFile) {
        if ($scope.uploadDocument.CommercialInvoice === '' &&
            $scope.uploadDocument.PackingList === '' &&
            $scope.uploadDocument.CustomDoc === '') {
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
            if (commercialInvoiceFile !== undefined && commercialInvoiceFile !== null) {
                files.push(commercialInvoiceFile);
            }
            if (packingListFile !== undefined && packingListFile !== null) {
                files.push(packingListFile);
            }
            if (customDocFile !== undefined && customDocFile !== null) {
                files.push(customDocFile);
            }
            $scope.upload = Upload.upload({
                url: config.SERVICE_URL + '/Shipment/UploadFile',
                fields: {
                    CommercialInvoiceFileName: $scope.uploadDocument.CommercialInvoiceFileName,
                    CommercialInvoice: $scope.uploadDocument.CommercialInvoice,
                    PackingListFileName: $scope.uploadDocument.PackingListFileName,
                    PackingList: $scope.uploadDocument.PackingList,
                    CustomDocFileName: $scope.uploadDocument.CustomDocFileName,
                    CustomDoc: $scope.uploadDocument.CustomDoc
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
                body: $scope.TitleFrayteValidation,
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

    function init() {

        // set Multilingual Modal Popup Options
        setModalOptions();
    }

    init();

});