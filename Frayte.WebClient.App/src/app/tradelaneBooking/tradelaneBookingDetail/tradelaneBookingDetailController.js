angular.module('ngApp.tradelaneBooking').controller('TradelaneBookingDetailController', function (AppSpinner, $http, config, Upload, UtilityService, $scope, $uibModalInstance, ShipmentId, TradelaneBookingService, $uibModalStack, $rootScope, $translate, $state, $filter, SessionService, $uibModal, toaster, CustomerService, UserService, ModalService, $window, PreAlertService, ModuleType) {

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
            'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
            'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess',
            'Confirmation', 'ShipmentCancelConfirmText', 'ReportCannotDownloadPleaseTryAgain', 'Could_Not_Download_TheReport', 'SuccessfullyDownloadSupplementaryChargePDF',
            'SendingEmail', 'CancellingTheShipment', 'For_Additional_Domestic', 'Visit', 'Read', 'UPS_Info', 'TNT_Info', 'NoDocumentAvailable', 'ErrorUploadingDocument',
            'DocumentAleadyUploadedFor', 'DocumentUploadedSuccessfully', 'TextPleaseSelectValidFile', 'FrayteError', 'FrayteWarning', 'FrayteSuccess', 'Successfully_Deleted_Document',
            'Error_Deleting_Document', 'Downloading', 'Document', 'Downloading_Document', 'Successfully_Download_Document', 'Couldnot_Download_Document']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.Success = translations.Success;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.FrayteErrorValidation = translations.FrayteError_Validation;
            $scope.ErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.GettingDetailsError = translations.GettingDetails_Error;
            $scope.CancelShipmentErrorValidation = translations.CancelShipmentError_Validation;
            $scope.GeneratePdfErrorValidation = translations.GeneratePdfError_Validation;
            $scope.SuccessfullySendlLabelValidation = translations.SuccessfullySendlLabel_Validation;
            $scope.SendingMailErrorValidation = translations.SendingMailError_Validation;
            $scope.EnterValidEmailAdd = translations.EnterValidEmailAdd;
            $scope.TrackShipmentNotTrackError = translations.TrackShipmentNotTrack_Error;
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
            $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
            $scope.Frayte_Error = translations.FrayteError;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.Confirmation = translations.Confirmation;
            $scope.ShipmentCancelConfirmText = translations.ShipmentCancelConfirmText;
            $scope.ReportCannotDownloadPleaseTryAgain = translations.ReportCannotDownloadPleaseTryAgain;
            $scope.Could_Not_Download_TheReport = translations.Could_Not_Download_TheReport;
            $scope.SuccessfullyDownloadSupplementaryChargePDF = translations.SuccessfullyDownloadSupplementaryChargePDF;
            $scope.SendingEmail = translations.SendingEmail;
            $scope.CancellingTheShipment = translations.CancellingTheShipment;
            $scope.ForAdditionalDomestic = translations.For_Additional_Domestic;
            $scope.VisitInfo = translations.Visit;
            $scope.ReadInfo = translations.Read;
            $scope.UPSInfo = translations.UPS_Info;
            $scope.TNTInfo = translations.TNT_Info;
            $scope.NoDocumentAvailable = translations.NoDocumentAvailable;
            $scope.ErrorUploadingDocument = translations.ErrorUploadingDocument;
            $scope.TextPleaseSelectValidFile = translations.TextPleaseSelectValidFile;
            $scope.DocumentUploadedSuccessfully = translations.DocumentUploadedSuccessfully;
            $scope.DocumentAleadyUploadedFor = translations.DocumentAleadyUploadedFor;
            $scope.Successfully_Deleted_Document = translations.Successfully_Deleted_Document;
            $scope.Error_Deleting_Document = translations.Error_Deleting_Document;
            $scope.Downloading = translations.Downloading;
            $scope.Document = translations.Document;
            $scope.Downloading_Document = translations.Downloading_Document;
            $scope.Successfully_Download_Document = translations.Successfully_Download_Document;
            $scope.Couldnot_Download_Document = translations.Couldnot_Download_Document;
        });
    };

    $scope.tradelaneClaim = function (shipment) {
        ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'tradelaneShipments/tradelaneShipmentsClaim/tradelaneShipmentClaim.tpl.html',
            controller: 'TradelaneClaimController',
            keyboard: true,
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ShipmentInfo: function () {
                    return $scope.tradelaneBookingDetail;
                }
            }
        });
    };

    // Document  Scrren

    $scope.showHAWBDetail = function (DocType, doc) {

        var str = doc.Document;

        if (doc.Document.search('.pdf') > -1 ||
            doc.Document.search('.xlsx') > -1 ||
            doc.Document.search('.xls') > -1) {
        }
        else {
            $scope.showPopUp(doc.Document);
        }
    };

    var getshipmentOtherDocuments = function (shipmetid, uploadType) {

        PreAlertService.getTradelaneDocuments($scope.shipmentId).then(function (response) {
            if (response.data && response.data.length) {
                $scope.otherDocuments = response.data[0].Documents;
                otherDocumentJson("type", uploadType);
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.GettingDetailsError,
                showCloseButton: true
            });
        });

    };

    var otherDocumentJson = function (type, UploadType) {

        $scope.attachments = [];

        if ($scope.documents.length) {

            if (type) {
                angular.forEach($scope.documents, function (obj) {
                    if (obj.DocumentType === "OtherDocument") {
                        obj.Documents = $scope.otherDocuments;
                    }
                });

                $scope.open = true;

                $scope.attachments = $scope.otherDocuments;
            }
            else {
                angular.forEach($scope.documents, function (obj) {
                    if (obj.DocumentType === "OtherDocument") {
                        angular.forEach(obj.Documents, function (obj1) {
                            $scope.attachments.push(obj1);
                        });
                    }
                });
            }
        }
    };

    var removeDocument = function (doc, document) {
        if ($scope.attachments && $scope.attachments.length) {
            for (var j = 0; j < $scope.attachments.length ; j++) {
                if ($scope.attachments[j].TradelaneShipmentDocumentId === doc.TradelaneShipmentDocumentId) {
                    $scope.attachments.splice(j, 1);
                    break;
                }
            }
        }
        if (!$scope.attachments.length) {
            $scope.open = false;
        }
        if (document.Documents.length) {
            for (var i = 0; i < document.Documents.length; i++) {
                if (document.Documents[i].TradelaneShipmentDocumentId === doc.TradelaneShipmentDocumentId) {
                    document.Documents.splice(i, 1);
                    break;
                }
            }
        }
    };

    $scope.removeDoc = function (doc, document) {
        console.log(doc);
        if (doc) {
            TradelaneBookingService.removeDocument(doc.TradelaneShipmentDocumentId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.Successfully_Deleted_Document,
                        showCloseButton: true
                    });
                    removeDocument(doc, document);
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.Error_Deleting_Document,
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.Error_Deleting_Document,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.Error_Deleting_Document,
                showCloseButton: true
            });
        }
    };

    $scope.WhileAddingExcel = function ($files, $file, $event) {
        if (!$file) {
            return;
        }
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        AppSpinner.showSpinnerTemplate($scope.UploadingDocument, $scope.Template);
        // Upload the excel file here.
        $scope.uploadExcel = Upload.upload({
            url: config.SERVICE_URL + '/TradelaneShipments/UploadOtherDocument',
            file: $file,
            fields: {
                ShipmentId: $scope.shipmentId,
                DocType: "OtherDocument",
                UserId: $scope.userId
            }
        });

        $scope.uploadExcel.progress($scope.progressExcel);

        $scope.uploadExcel.success($scope.successExcel);

        $scope.uploadExcel.error($scope.errorExcel);
    };

    $scope.progressExcel = function (evt) {
        //To Do:  show excel uploading progress message 
    };
    $scope.successExcel = function (data, status, headers, config) {
        if (status = 200) {
            if (data) {
                if (data === "Ok") {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.DocumentUploadedSuccessfully,
                        showCloseButton: true
                    });


                    getshipmentOtherDocuments($scope.shipmentId, "Upload");
                }
                else if (data === "Failed") {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.ErrorUploadingDocument,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.DocumentAleadyUploadedFor + data,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: data.Message,
                    showCloseButton: true
                });
                AppSpinner.hideSpinnerTemplate();
            }
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorUploadingDocument,
                showCloseButton: true
            });
        }
    };

    $scope.errorExcel = function (err) {
        AppSpinner.hideSpinnerTemplate();
        toaster.pop({
            type: 'error',
            title: $scope.FrayteError,
            body: $scope.Errorwhil_uploading_the_excel,
            showCloseButton: true
        });
    }; 

    $scope.detailScreen = function () {
        $scope.tabType = "Detail";
    };
    $scope.documentScreen = function () {

        if ($scope.tradelaneBookingDetail.ShipmentStatusId !== 29) {

            $scope.tabType = "Document";

            TradelaneBookingService.TradelaneShipmentDocuments($scope.userInfo.EmployeeId, $scope.shipmentId).then(function (response) {
                if (response.data && response.data.length) {

                    $scope.documents = response.data; 
                    otherDocumentJson(); 
                    setHAWBDocumentOrder(); 
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.NoDocumentAvailable,
                        showCloseButton: true
                    });
                } 
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.GettingDetailsError,
                    showCloseButton: true
                });
            });
        } 
    }; 

    $scope.downLaodHAWB = function (HAWB) {
        $scope.dowonLoadDoc("HAWB", HAWB);
    };

    $scope.dowonLoadDoc = function (docType, docTypeName) {
        $rootScope.GetServiceValue = null;
        var value = "";

        if (docType === "HAWB") {
            value = " HAWB ";
        }
        else if (docType === "MAWB") {
            value = " MAWB ";
        }
        else if (docType === "CoLoadForm") {
            value = " CoLoad Form ";
        }
        else if (docType === "Manifest") {
            value = " Manifest ";
        }
        else if (docType === "ShipmentDetail") {
            value = " Shipment Detail ";
        }
        else {
            value = "";
        }
        var message = $scope.Downloading + value + $scope.Document;

        AppSpinner.showSpinnerTemplate(message, $scope.Template);

        TradelaneBookingService.CreateDocument($scope.shipmentId, $scope.userInfo.EmployeeId, docType, docTypeName).then(function (response) {
            if (response.data !== null) {
                var fileInfo = response.data;
                var File = {
                    TradelaneShipmentId: $scope.shipmentId,
                    FileName: response.data.FileName
                };
                if (response.data != null) {
                    $http({
                        method: 'POST',
                        url: config.SERVICE_URL + '/TradelaneShipments/DownloadDocument',
                        data: File,
                        responseType: 'arraybuffer'
                    }).success(function (data, status, headers) {
                        if (status == 200 && data !== null) {
                            headers = headers();
                            var filename = headers['x-filename'];
                            var contentType = headers['content-type'];

                            var linkElement = document.createElement('a');
                            try {
                                var blob = new Blob([data], { type: contentType });
                                var url = window.URL.createObjectURL(blob);

                                linkElement.setAttribute('href', url);
                                if (filename === undefined || filename === null) {
                                    linkElement.setAttribute("download", "Generated_Report." + fileType);
                                }
                                else {
                                    linkElement.setAttribute("download", filename);
                                }

                                var clickEvent = new MouseEvent("click", {
                                    "view": window,
                                    "bubbles": true,
                                    "cancelable": false
                                });
                                linkElement.dispatchEvent(clickEvent);
                                AppSpinner.hideSpinnerTemplate();

                            } catch (ex) {
                                AppSpinner.hideSpinnerTemplate();
                                $window.open(fileInfo.FilePath, "_blank");
                                toaster.pop({
                                    type: 'success',
                                    title: $scope.FrayteSuccess,
                                    body: $scope.Successfully_Download_Document,
                                    showCloseButton: true
                                });
                            }

                        }
                    })
                   .error(function (data) {
                       AppSpinner.hideSpinnerTemplate();
                       toaster.pop({
                           type: 'error',
                           title: $scope.FrayteError,
                           body: $scope.Couldnot_Download_Document,
                           showCloseButton: true
                       });
                   });

                }
                else {
                }
            }
             
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.Couldnot_Download_Document,
                showCloseButton: true
            });
        });
    };

    // End Document  Scrren
     
    $scope.GetCorrectFormattedDatePanel = function (date) {
        if (date) {
            return UtilityService.GetForMattedDate(date);
        }
    };

    $scope.GetCorrectFormattedDate = function (date) {
        // Geting Correct Date Format
        if (date !== null && date !== '' && date !== undefined) {
            var newDate = new Date(date);
            Number.prototype.padLeft = function (base, chr) {
                var len = (String(base || 10).length - String(this).length) + 1;
                return len > 0 ? new Array(len).join(chr || '0') + this : this;
            };

            var d = newDate;
            var Mon = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var day = d.getDate();
            var Month = d.getMonth();
            var Month1 = Mon[Month];
            var Year = d.getFullYear();
            var dformat = day + "-" + Month1 + "-" + Year;
            return dformat;
        }
        else {
            return;
        }
    };
     
    $scope.totalWeight = function () {
        if ($scope.tradelaneBookingDetail && $scope.tradelaneBookingDetail.HAWBPackages && $scope.tradelaneBookingDetail.HAWBPackages.length) {
            var sum = 0;
            for (var i = 0; i < $scope.tradelaneBookingDetail.HAWBPackages.length; i++) {
                sum += Math.abs(parseInt($scope.tradelaneBookingDetail.HAWBPackages[i].EstimatedWeight, 10));
            }
            return sum;
        }
        else {
            return 0;
        }
    };
    $scope.totalCarton = function () {
        if ($scope.tradelaneBookingDetail && $scope.tradelaneBookingDetail.HAWBPackages && $scope.tradelaneBookingDetail.HAWBPackages.length) {
            var sum = 0;
            for (var i = 0; i < $scope.tradelaneBookingDetail.HAWBPackages.length; i++) {
                sum += Math.abs(parseInt($scope.tradelaneBookingDetail.HAWBPackages[i].TotalCartons, 10));
            }
            return sum;
        }
        else {
            return 0;
        }
    };

    $scope.showPopUp = function (HAWB) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'tradelaneBooking/tradelaneBookingDetail/tradelaneBookingDetailPackageDetail.tpl.html',
            controller: 'TradelaneBookingDetailPacakgeDetailController',
            windowClass: 'directBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                ShipmentId: function () {
                    return $scope.shipmentId;
                },
                PackageCalculatonType: function () {
                    return $scope.tradelaneBookingDetail.PakageCalculatonType;
                },
                FrayteNumber: function () {
                    return $scope.tradelaneBookingDetail.FrayteNumber;
                },
                HAWB: function () {
                    if (HAWB) {
                        return HAWB;
                    }
                    else {
                        return "";
                    }
                },
                HAWBNumber: function () {
                    return $scope.tradelaneBookingDetail.HAWBNumber;
                },
                TotalUploaded: function () {
                    return 0;
                },
                SuccessUploaded: function () {
                    return 0;
                },
                ScreenType: function () {
                    return "ShipmentDetail";
                }
            }
        });
        modalInstance.result.then(function (response) {

        }, function () {
        });
    };

    var getTotalShipments = function () {
        TradelaneBookingService.IsAllHawbAssigned($scope.shipmentId).then(function (response) {
            if (response.data) {
                $scope.Booking = response.data;
            }
            else {
                $scope.IsAllHawbAssigned = false;
            }
        }, function () {
            $scope.IsAllHawbAssigned = false;
        });
    };

    function compare(a, b) {
        if (!isNaN(parseInt(a.HAWB, 10)) && !isNaN(parseInt(b.HAWB, 10))) {
            if (parseInt(a.HAWB, 10) < parseInt(b.HAWB, 10))
            { return -1; }
            if (parseInt(a.HAWB, 10) > parseInt(b.HAWB, 10))
            { return 1; }
        }
        return 0;
    }

    var setHAWBDocumentOrder = function () {
        $scope.tradelaneBookingDetail.HAWBPackages.sort(compare);
    };

    var getCustomerDetail = function () { 
        CustomerService.GetCustomerDetail($scope.tradelaneBookingDetail.CustomerId).then(function (response) {
            $scope.customerDetail = response.data;
            var dbr = $scope.customerDetail.AccountNumber.split("");
            var accno = "";
            for (var j = 0; j < dbr.length; j++) {
                accno = accno + dbr[j];
                if (j == 2 || j == 5) {
                    accno = accno + "-";
                }
            }
            $scope.customerDetail.AccountNumber = accno;
        }, function () {
            console.log("Could not get customer detail.");
        });

    };

    var getTradelaneBookingDetail = function () {
        CallingType = "";
        TradelaneBookingService.GetBookingDetail($scope.shipmentId, '').then(function (response) {

            $scope.tradelaneBookingDetail = response.data;
            if ($scope.RoleId && ($scope.RoleId === 1 || $scope.RoleId === 6)) {
                getCustomerDetail();
            }
            $scope.tradelaneBookingDetail.CreatedOnNxtSevenDay = new Date(new Date($scope.tradelaneBookingDetail.CreatedOnUtc).setDate(new Date($scope.tradelaneBookingDetail.CreatedOnUtc).getDate() + 7));
            $scope.IsClaim = false;
            if (new Date() <= $scope.tradelaneBookingDetail.CreatedOnNxtSevenDay) {
                $scope.IsClaim = true;
            }
            if (!parseFloat($scope.tradelaneBookingDetail.DeclaredValue)) {
                $scope.tradelaneBookingDetail.DeclaredValue = null;
            }
            if ($scope.tradelaneBookingDetail.ShipmentStatusId !== 27) {
                getTotalShipments();
                setHAWBDocumentOrder();
            }
            else {
                getTotalShipments();
            }
            if ($scope.tradelaneBookingDetail.PakageCalculatonType = "kgToCms") {

                $translate('kGS').then(function (kGS) {
                    $scope.Lb_Kgs = kGS;
                });
                $translate('KG').then(function (KG) {

                    $scope.Lb_Kg = KG;
                });

            }
            else if ($scope.tradelaneBookingDetail.PakageCalculatonType = "lbToInchs") {

                $translate('LB').then(function (LB) {
                    $scope.Lb_Kgs = LB;
                });
                $translate('LBs').then(function (LBs) {

                    $scope.Lb_Kg = LBs;
                });
                $translate('INCHS').then(function (Inchs) {
                    $scope.Lb_Inch = Inchs;
                });

            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.GettingDetailsError,
                showCloseButton: true
            });
        });
    };

    $scope.submit = function () {
        $uibModalInstance.close();
    };

    $scope.closePage = function () {
        $uibModalInstance.close();
    };
    $scope.GetCorrectFormattedDate = function (date) {
        // Geting Correct Date Format
        if (date !== null && date !== '' && date !== undefined) {
            var newDate = new Date(date);
            Number.prototype.padLeft = function (base, chr) {
                var len = (String(base || 10).length - String(this).length) + 1;
                return len > 0 ? new Array(len).join(chr || '0') + this : this;
            };

            var d = newDate;
            var Mon = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var day = d.getDate();
            var Month = d.getMonth();
            var Month1 = Mon[Month];
            var Year = d.getFullYear();
            var dformat = day + "-" + Month1 + "-" + Year;
            return dformat;
        }
        else {
            return;
        }
    };

    $scope.GetCorrectFormattedTime = function (Time) {
        var ForMatedTime = "";
        if (Time !== undefined && Time !== null) {

            var T = Time.split('');

            for (i = 0; i < T.length; i++) {
                ForMatedTime = ForMatedTime + T[i];
                if (i === 1) {
                    ForMatedTime = ForMatedTime + ":";
                }
            }
        }
        return ForMatedTime;
    };

    $scope.cloneShipment = function () {
        $state.go("loginView.userTabs.tradelane-booking-clone", { shipmentId: $scope.tradelaneBookingDetail.TradelaneShipmentId }, { reload: true });
        $uibModalStack.dismissAll();
    };

    $scope.totalShipments = function () {
        if ($scope.tradelaneBookingDetail && $scope.tradelaneBookingDetail.HAWBPackages) {
            if ($scope.tradelaneBookingDetail.HAWBPackages && $scope.tradelaneBookingDetail.HAWBPackages.length) {
                var sum = 0;
                for (var i = 0; i < $scope.tradelaneBookingDetail.HAWBPackages.length; i++) {

                    sum += $scope.tradelaneBookingDetail.HAWBPackages[i].Packages.length;
                }
                return sum;
            }
            else {
                return 0;
            }
        }


    };
     
    function init() {

        $scope.tabType = "Detail";
        $scope.url = SessionService.GetSiteURL();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        setMultilingualOptions();
        $scope.buildURL = config.BUILD_URL;
        $scope.IsCloneBtnShow = false;
        if (ModuleType !== undefined && ModuleType !== null && ModuleType !== "" && ModuleType === "ExpressBooking") {
            $scope.tabType = "";
        }

        $scope.userInfo = SessionService.getUser();
        if ($scope.userInfo !== undefined) {
            $scope.userId = $scope.userInfo.EmployeeId;
            $scope.RoleId = $scope.userInfo.RoleId;
            $scope.shipmentId = ShipmentId;
            getTradelaneBookingDetail();
        }
    }

    init();
});
