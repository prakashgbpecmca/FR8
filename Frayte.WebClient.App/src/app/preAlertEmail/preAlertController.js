angular.module('ngApp.preAlert').controller('PreAlertEmailController', function (AppSpinner, Upload, UtilityService, $scope, $uibModalInstance, ShipmentId, config, PreAlertService, $uibModalStack, $rootScope, $translate, $state, TradelaneBookingService, SessionService, $uibModal, toaster, ModalService, $window, CustomerService) {

    $scope.GetCorrectFormattedDatePanel = function (date) {
        if (date) {
            return UtilityService.GetForMattedDate(date);
        }
    };

    $scope.closePage = function () {
        $uibModalInstance.close();
    };


    //other document toggle
    $scope.otherDocumentToggle = function () {
        $scope.open = !$scope.open;
    };
    $scope.shipmentOpen = function () {
        $scope.shipmentOpened = !$scope.shipmentOpened;
    };
    //end

    // translation key code
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'SureToDeleteIt', 'Confirmation', 'SendingPreAlertEmail', 'SuccessfullySentPreAlertEmail',
        'Error_While_Sending_Email', 'CorrectValidationErrorFirst', 'Something_Bad_Happen', 'File_Aleady_Uploaded_For', 'DocumentUploadedSuccessfully', 'ErrorUploadingDocument',
        'UploadingDocument', 'ThereSomeErrorOccured', 'Errorwhil_uploading_the_excel', 'DocumentAleadyUploadedFor', 'Loading_PreAlert_Email']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.Confirmation = translations.Confirmation;
            $scope.SureToDeleteIt = translations.SureToDeleteIt;
            $scope.ThereSomeErrorOccured = translations.ThereSomeErrorOccured;
            $scope.UploadingDocument = translations.UploadingDocument;
            $scope.ErrorUploadingDocument = translations.ErrorUploadingDocument;
            $scope.DocumentUploadedSuccessfully = translations.DocumentUploadedSuccessfully;
            $scope.File_Aleady_Uploaded_For = translations.File_Aleady_Uploaded_For;
            $scope.Something_Bad_Happen = translations.Something_Bad_Happen;
            $scope.CorrectValidationErrorFirst = translations.CorrectValidationErrorFirst;
            $scope.Error_While_Sending_Email = translations.Error_While_Sending_Email;
            $scope.SuccessfullySentPreAlertEmail = translations.SuccessfullySentPreAlertEmail;
            $scope.SendingPreAlertEmail = translations.SendingPreAlertEmail;
            $scope.Errorwhil_uploading_the_excel = translations.Errorwhil_uploading_the_excel;
            $scope.DocumentAleadyUploadedFor = translations.DocumentAleadyUploadedFor;
            $scope.Loading_PreAlert_Email = translations.Loading_PreAlert_Email;

            getTradelaneBookingDetail();

        });
    };

    // Add Remove Email
    var removeEmailFromPreAlerEmailTo = function (email) {
        var index = $scope.preAlertDetail.PreAlerEmailTo.indexOf(email);
        $scope.preAlertDetail.PreAlerEmailTo.splice(index, 1);
    };

    $scope.removeMail = function (email, Isvalid) {
        if (email) {
            if (Isvalid) {
                var modalOptions = {
                    headerText: $scope.Confirmation,
                    bodyText: $scope.SureToDeleteIt
                };
                ModalService.Confirm({}, modalOptions).then(function (result) {
                    removeEmailFromPreAlerEmailTo(email);
                    if (!$scope.preAlertDetail.PreAlerEmailTo.length) {
                        $scope.addEmail();
                    }
                });
            }
            else {
                removeEmailFromPreAlerEmailTo(email);
                if (!$scope.preAlertDetail.PreAlerEmailTo.length) {
                    $scope.addEmail();
                }
            }
        }
    };

    $scope.IsEmailToExist = function (email) {
        $scope.IsToEmailExist = false;
        for (var i = 0; i < $scope.preAlertDetail.PreAlerEmailTo.length; i++) {

            // trim the emails 
            if ($scope.preAlertDetail.PreAlerEmailTo[i].Email === email.Email) {
                $scope.IsToEmailExist = true;
                break;
            }
        }
    };

    $scope.addEmail = function () {
        var email = {
            TradelaneUserTrackingConfigurationDetailId: 0,
            Name: '',
            Email: ''
        };
        $scope.preAlertDetail.PreAlerEmailTo.push(email);
    };

    var removeEmailFromPreAlerEmailCC = function (email) {
        var index = $scope.preAlertDetail.PreAlerEmailCC.indexOf(email);
        $scope.preAlertDetail.PreAlerEmailCC.splice(index, 1);
    };

    $scope.removeMailCC = function (email, Isvalid) {
        if (email) {
            if (Isvalid && email.Email) {
                var modalOptions = {
                    headerText: $scope.Confirmation,
                    bodyText: $scope.SureToDeleteIt
                };
                ModalService.Confirm({}, modalOptions).then(function (result) {
                    removeEmailFromPreAlerEmailCC(email);
                    if (!$scope.preAlertDetail.PreAlerEmailCC.length) {
                        $scope.addEmailCC();
                    }
                });
            }
            else {
                removeEmailFromPreAlerEmailCC(email);
                if (!$scope.preAlertDetail.PreAlerEmailCC.length) {
                    $scope.addEmailCC();
                }
            }
        }
    };

    $scope.addEmailCC = function () {
        var email = {
            TradelaneUserTrackingConfigurationDetailId: 0,
            Name: '',
            Email: ''
        };
        $scope.preAlertDetail.PreAlerEmailCC.push(email);
    };

    var removeEmailFromPreAlerEmailBCC = function (email) {
        var index = $scope.preAlertDetail.PreAlerEmailBCC.indexOf(email);
        $scope.preAlertDetail.PreAlerEmailBCC.splice(index, 1);
    };

    $scope.removeMailBCC = function (email, Isvalid) {
        if (email) {
            if (Isvalid && email.Email) {
                var modalOptions = {
                    headerText: $scope.Confirmation,
                    bodyText: $scope.SureToDeleteIt
                };
                ModalService.Confirm({}, modalOptions).then(function (result) {
                    removeEmailFromPreAlerEmailBCC(email);
                    if (!$scope.preAlertDetail.PreAlerEmailBCC.length) {
                        $scope.addEmailBCC();
                    }
                });
            }
            else {
                removeEmailFromPreAlerEmailBCC(email);
                if (!$scope.preAlertDetail.PreAlerEmailBCC.length) {
                    $scope.addEmailBCC();
                }
            }
        }
    };

    $scope.addEmailBCC = function () {
        var email = {
            TradelaneUserTrackingConfigurationDetailId: 0,
            Name: '',
            Email: ''
        };
        $scope.preAlertDetail.PreAlerEmailBCC.push(email);
    };

    var getDocuments = function () {
        $scope.open = true;

        PreAlertService.getTradelaneDocuments($scope.shipmentId).then(function (response) {
            var flag = false;
            if (response.data && response.data.length && response.data[0].Documents && response.data[0].Documents.length) {
                angular.forEach(response.data[0].Documents, function (obj) {
                    var bool = false;
                    if ($scope.masterDataOtherDocuments && $scope.masterDataOtherDocuments.length) {
                        for (var i = 0; i < $scope.masterDataOtherDocuments.length ; i++) {
                            if ($scope.masterDataOtherDocuments[i].TradelaneShipmentDocumentId == obj.TradelaneShipmentDocumentId) {
                                bool = true;
                            }
                        }
                        if (!bool) {
                            obj.IsSelected = true;
                        }
                    }
                    else {
                        obj.IsSelected = true;
                    }
                    if ($scope.otherDocuments && $scope.otherDocuments[0]) {

                        $scope.otherDocuments[0].Documents.push(obj);

                    }

                });
                angular.forEach($scope.preAlertDetail.PreAlertDocumnets, function (obj) {
                    if (obj.DocumentType === "OtherDocument") {
                        obj.Documents = response.data[0].Documents;
                        flag = true;
                        if ($scope.masterDataOtherDocuments && $scope.masterDataOtherDocuments.length) {
                            angular.forEach(obj.Documents, function (obj1) {
                                for (var i = 0; i < $scope.masterDataOtherDocuments.length ; i++) {
                                    if ($scope.masterDataOtherDocuments[i].TradelaneShipmentDocumentId === obj1.TradelaneShipmentDocumentId) {
                                        if ($scope.masterDataOtherDocuments[i].IsSelected) {
                                            obj1.IsSelected = true;
                                            break;
                                        }
                                    }
                                }
                            });
                        }
                    }
                });
                if (!flag) {
                    $scope.preAlertDetail.PreAlertDocumnets.push(response.data[0]);
                    $scope.masterData = null;
                    $scope.masterData = angular.copy($scope.preAlertDetail);

                    otherDocumentJson("type");
                }
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.ThereSomeErrorOccured,
                showCloseButton: true
            });
        });
    };

    $scope.WhileAddingExcel = function ($files, $file, $event) {
        if (!$file) {
            return;
        }
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
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

                    for (var i = 0; i < $scope.preAlertDetail.PreAlertDocumnets.length ; i++) {
                        if ($scope.preAlertDetail.PreAlertDocumnets[i].DocumentType === "OtherDocument") {
                            if ($scope.preAlertDetail.PreAlertDocumnets[i].Documents && $scope.preAlertDetail.PreAlertDocumnets[i].Documents.length) {
                                $scope.masterDataOtherDocuments = angular.copy($scope.preAlertDetail.PreAlertDocumnets[i].Documents);
                                break;
                            }
                        }
                    }
                    getDocuments($scope.shipmentId);
                }
                else if (data === "Failed") {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.ErrorUploadingDocument,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteWarning,
                        body: $scope.DocumentAleadyUploadedFor + data,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
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
                title: $scope.TitleFrayteError,
                body: $scope.ErrorUploadingDocument,
                showCloseButton: true
            });
        }
    };

    $scope.errorExcel = function (err) {
        AppSpinner.hideSpinnerTemplate();
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scope.Errorwhil_uploading_the_excel,
            showCloseButton: true
        });
    };

    var removeDocFromUI = function (doc, document) {
        if ($scope.otherDocuments && $scope.otherDocuments[0].Documents.length) {
            for (var j = 0; j < $scope.otherDocuments[0].Documents.length ; j++) {
                if ($scope.otherDocuments[0].Documents[j].TradelaneShipmentDocumentId === doc.TradelaneShipmentDocumentId) {
                    $scope.otherDocuments[0].Documents.splice(j, 1);
                    break;
                }
            }
        }
        if (!$scope.otherDocuments.length) {
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

    $scope.removeOtherDoc = function (doc, document) {
        if (doc) {
            PreAlertService.removeOtherDoc(doc.TradelaneShipmentDocumentId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: "Frayte-Success",
                        body: "Successfully removed the document.",
                        showCloseButton: true
                    });
                    removeDocFromUI(doc, document);
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: "Frayte-Error",
                    body: "Error while removing document.",
                    showCloseButton: true
                });
            });
        }
    };

    $scope.sendEmail = function (IsValid) {
        if (IsValid) {

            AppSpinner.showSpinnerTemplate($scope.SendingPreAlertEmail, $scope.Template);
            PreAlertService.SendPreAlertEmail($scope.preAlertDetail).then(function (response) {
                if (response.data && response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.SuccessfullySentPreAlertEmail,
                        showCloseButton: true
                    });

                    $uibModalInstance.close();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.Error_While_Sending_Email,
                        showCloseButton: true
                    });
                }

                AppSpinner.hideSpinnerTemplate();
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.Error_While_Sending_Email,
                    showCloseButton: true
                });
                AppSpinner.hideSpinnerTemplate();
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
        }
    };

    var otherDocumentJson = function (type) {
        if (!type) {
            // Pre select some of the document
            if ($scope.preAlertDetail.PreAlertDocumnets && $scope.preAlertDetail.PreAlertDocumnets.length) {
                angular.forEach($scope.preAlertDetail.PreAlertDocumnets, function (obj) {
                    if (obj.DocumentType === "MAWB" || obj.DocumentType === "HAWB" || obj.DocumentType === "Manifest" || obj.DocumentType === "CoLoadForm") {
                        if (obj.Documents && obj.Documents.length) {
                            angular.forEach(obj.Documents, function (obj1) {
                                obj1.IsSelected = true;
                            });
                        }
                    }
                });
            }
        }

        // Special work for other document section
        $scope.otherDocuments = [];

        if ($scope.masterData.PreAlertDocumnets.length) {
            angular.forEach($scope.masterData.PreAlertDocumnets, function (obj) {
                if (obj.DocumentType === "OtherDocument") {
                    var oj = {
                        DocumentType: "",
                        DocumentTypeDisplay: "",
                        Documents: []
                    };
                    oj.DocumentType = obj.DocumentType;
                    obj.DocumentTypeDisplay = obj.DocumentTypeDisplay;

                    if (obj.Documents && obj.Documents.length) {
                        //angular.forEach(obj.Documents, function (obj1) {
                        //    obj.Documents.push(obj1);
                        //});
                        $scope.otherDocuments.push(obj);
                    }
                }
            });

            $scope.attachments = [];
            angular.forEach($scope.masterData.PreAlertDocumnets, function (obj) {
                if (obj.DocumentType !== "OtherDocument") {
                    var oj = {
                        DocumentType: "",
                        DocumentTypeDisplay: "",
                        Documents: []
                    };
                    oj.DocumentType = obj.DocumentType;
                    obj.DocumentTypeDisplay = obj.DocumentTypeDisplay;

                    if (obj.Documents && obj.Documents.length) {
                        angular.forEach(obj.Documents, function (obj1) {
                            obj.Documents.push(obj1);
                        });
                        $scope.attachments.push(obj);
                    }
                }
            });
        }
    };

    var getCustomerDetail = function (tradelaneBookingDetail) {
        CustomerService.GetCustomerDetail($scope.tradelaneBookingDetail.CustomerId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.data) {
                $scope.CustomerDetail = response.data;
                //$scope.preAlertDetail.EmailSubject = $scope.CustomerDetail.CompanyName + " - Tradelane Shipment Pre-Alert - " + tradelaneBookingDetail.FrayteNumber;
                var Mawbvar = "";
                if (tradelaneBookingDetail.MAWB !== undefined && tradelaneBookingDetail.MAWB !== null && tradelaneBookingDetail.MAWB !== "") {
                    Mawbvar = "MAWB# " + tradelaneBookingDetail.AirlinePreference.AilineCode + " " + tradelaneBookingDetail.MAWB.substring(0, 4) + " " + tradelaneBookingDetail.MAWB.substring(4, 8);
                }
                else {
                    Mawbvar = tradelaneBookingDetail.FrayteNumber;
                }
                $scope.preAlertDetail.EmailSubject = Mawbvar + " (" + tradelaneBookingDetail.DepartureAirport.AirportCode + " To " + tradelaneBookingDetail.DestinationAirport.AirportCode + ")" + " - " + $scope.CustomerDetail.CompanyName + ": " + $scope.CustomerDetail.ContactName + " - Shipment Pre-Alert";
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    };

    var getTradelaneBookingDetail = function () {
        CallingType = "";
        AppSpinner.showSpinnerTemplate($scope.Loading_PreAlert_Email, $scope.Template);
        TradelaneBookingService.GetBookingDetail($scope.shipmentId, '').then(function (response) {
            $scope.tradelaneBookingDetail = response.data;
            $scope.totalweight = 0;
            $scope.totalCartons = 0;
            $scope.totalVolume = 0;
            for (i = 0; i < $scope.tradelaneBookingDetail.HAWBPackages.length; i++) {
                // $scope.tradelaneBookingDetail.HAWBPackages[i].TotalVolume = $scope.tradelaneBookingDetail.HAWBPackages[i].TotalVolume.toFixed(2);   
                $scope.totalweight = $scope.totalweight + $scope.tradelaneBookingDetail.HAWBPackages[i].TotalWeight;
                $scope.totalVolume = $scope.totalVolume + $scope.tradelaneBookingDetail.HAWBPackages[i].TotalVolume;
                $scope.totalCartons = $scope.totalCartons + $scope.tradelaneBookingDetail.HAWBPackages[i].TotalCartons;
            }
            $scope.totalVolume = $scope.totalVolume.toFixed(2);
            getScreenInitials();

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
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteErrorValidation,
                body: $scope.GettingDetailsError,
                showCloseButton: true
            });
        });
    };

    var getScreenInitials = function () {
        PreAlertService.PreALertInitials($scope.shipmentId).then(function (response) {
            if (response.data) {
                $scope.preAlertDetail = response.data;
                $scope.preAlertDetail.UserId = $scope.userId;
                $scope.preAlertDetail.TradelaneShipmentId = $scope.shipmentId;
                $scope.preAlertDetail.EmailBody = "";
                $scope.masterData = angular.copy($scope.preAlertDetail);

                otherDocumentJson();

                if (!$scope.preAlertDetail.PreAlerEmailTo || !$scope.preAlertDetail.PreAlerEmailTo.length) {
                    $scope.addEmail();
                }
                if (!$scope.preAlertDetail.PreAlerEmailCC) {
                    $scope.preAlertDetail.PreAlerEmailCC = [];
                    $scope.addEmailCC();
                }
                if (!$scope.preAlertDetail.PreAlerEmailBCC) {
                    $scope.preAlertDetail.PreAlerEmailBCC = [];
                    $scope.addEmailBCC();
                }
                getCustomerDetail($scope.tradelaneBookingDetail);
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.Something_Bad_Happen,
                showCloseButton: true
            });
        });

    };

    function init() {
        $scope.url = SessionService.GetSiteURL();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.emailFormat = /^[a-z0-9._]+@[a-z]+\.[a-z.]/;
        $scope.submitted = true;
        $scope.open = false;
        $scope.shipmentOpened = false;

        $scope.SiteAddress = config.Public_Link;

        $scope.userInfo = SessionService.getUser();
        if ($scope.userInfo) {
            setModalOptions();
            $scope.userId = $scope.userInfo.EmployeeId;
            $scope.shipmentId = ShipmentId;
        }
        else {
            $state.go("login");
        }
    }

    init();
});
