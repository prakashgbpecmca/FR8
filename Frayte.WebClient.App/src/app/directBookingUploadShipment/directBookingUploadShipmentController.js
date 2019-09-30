angular.module('ngApp.directBookingUploadShipment').controller('DirectBookingUploadShipment', function ($scope, $rootScope, toaster, $translate, Upload, uiGridConstants, DownloadExcelService, CustomerService, config, $state, SessionService, UploadShipmentService, $uibModal, $http, $window, AppSpinner, ModalService, $interval, $timeout, DbUploadShipmentService, DirectBookingService) {
    $scope.status = {
        isCustomHeaderOpen: false,
        isFirstOpen: true,
        isFirstDisabled: false
    };

    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'Frayte_Success', 'FrayteWarning',
                    'PleaseCorrectValidationErrors', 'PleaseSelectValidFile', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload',
                    'ErrorGettingShipmentDetailServer', 'FrayteWarning_Validation', 'InitialData_Validation', 'DownloadSuccessfull', 'Created_Shipment',
                    'select_atleast_one', 'ShipmentNot_Avaliable', 'Invalid_Excel_File_Missing_Required', 'MissingInformationInSomeSessionShipment', 'ShipmentDeletedFromSession', 'Download_Shipments',
                    'Reupload_Place_Booking', 'Sure_To_Delete_Shipment_Draft', 'Shipment_Delete_Confirmation', 'Upload_SuccessfullMessage', 'ShipmentLabelPrintedIn30Minutes',
                    'ServiceCodeSavedSuccessfully', 'ErrorPrintingLabels', 'ErrorSavingServiceCode', 'UploadingDirectBookingShipments', 'RemovingShipments', 'WithServiceShipments', 'WeUploadingYourShipments',
                    'FromCountryToCountryNameMissing', 'Shipment_Is', 'Shipment_Are']).then(function (translations) {
                    $scope.Frayte_Warning = translations.FrayteWarning;
                    $scope.Frayte_Success = translations.Frayte_Success;
                    $scope.ShipmentDeletedFromSession = translations.ShipmentDeletedFromSession;
                    $scope.TitleFrayteError = translations.FrayteError;
                    $scope.TitleFrayteInformation = translations.FrayteInformation;
                    $scope.TitleFrayteValidation = translations.FrayteValidation;
                    $scope.TitleFrayteSuccess = translations.FrayteSuccess;
                    $scope.TextValidation = translations.PleaseCorrectValidationErrors;
                    $scope.TextPleaseSelectValidFile = translations.PleaseSelectValidFile;
                    $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
                    $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
                    $scope.TextErrorGettingShipmentDetailServer = translations.ErrorGettingShipmentDetailServer;
                    $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
                    $scope.InitialDataValidation = translations.InitialData_Validation;
                    $scope.DownloadSuccessfullCsv = translations.DownloadSuccessfull;
                    $scope.CreatedShipment = translations.Created_Shipment;
                    $scope.selectatleastone = translations.select_atleast_one;
                    $scope.ShipmentNotAvaliable = translations.ShipmentNot_Avaliable;
                    $scope.Invalid_Excel_File_Missing_Required = translations.Invalid_Excel_File_Missing_Required;
                    $scope.MissingInformationInSomeSessionShipment = translations.MissingInformationInSomeSessionShipment;
                    $scope.Download_Shipments = translations.Download_Shipments;
                    $scope.Reupload_Place_Booking = translations.Reupload_Place_Booking;
                    $scope.Shipment_Delete_Confirmation = translations.Shipment_Delete_Confirmation;
                    $scope.Sure_To_Delete_Shipment_Draft = translations.Sure_To_Delete_Shipment_Draft;
                    $scope.UploadSuccessfullMessage = translations.Upload_SuccessfullMessage;
                    $scope.ShipmentLabelPrintedIn30Minutes = translations.ShipmentLabelPrintedIn30Minutes;
                    $scope.ServiceCodeSavedSuccessfully = translations.ServiceCodeSavedSuccessfully;
                    $scope.ErrorSavingServiceCode = translations.ErrorSavingServiceCode;
                    $scope.ErrorPrintingLabels = translations.ErrorPrintingLabels;
                    $scope.UploadingDirectBookingShipments = translations.UploadingDirectBookingShipments;
                    $scope.RemovingShipments = translations.RemovingShipments;
                    $scope.WithServiceShipments = translations.WithServiceShipments;
                    $scope.WeUploadingYourShipments = translations.WeUploadingYourShipments;
                    $scope.FromCountryToCountryNameMissing = translations.FromCountryToCountryNameMissing;
                    $scope.ShipmentIs = translations.Shipment_Is;
                    $scope.ShipmentAre = translations.Shipment_Are;
        });
    };

    // Uplaod agnets via excel
    $scope.WhileAddingReceiverExcel = function ($files, $file, $event) {
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        // Upload the excel file here.
        $scope.uploadExcel = Upload.upload({

            url: config.SERVICE_URL + '/DirectBookingUploadShipment/UploadShipments',

            params: {
                CustomerId: $scope.CustomerId,
                LogisticService: $scope.LogisticService,
                ServiceType: $scope.ServiceType
            },
            file: $file
        });

        $scope.uploadExcel.progress($scope.progressExcel);

        $scope.uploadExcel.success($scope.successExcel);

        $scope.uploadExcel.error($scope.errorExcel);
    };

    $scope.progressExcel = function (evt) {
        AppSpinner.showSpinnerTemplate($scope.WeUploadingYourShipments, $scope.Template);
        //AppSpinner.hideSpinnerTemplate();
        //console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
        //toaster.clear();
        //toaster.pop({
        //    type: 'success',
        //    title: 'uploading',
        //    body: 'percent: ' + parseInt(100.0 * evt.loaded / evt.total, 10),
        //    showCloseButton: true
        //});
    };

    $scope.successExcel = function (data, status, headers, config) {
        if (status = 200) {
            AppSpinner.hideSpinnerTemplate();
            $scope.flag1 = false;
            //$scope.WithServiceShipments();
            //$scope.GetSessionShipments($scope.SessionRecord);
            //$scope.GetSessionDropLastRecord();
            $scope.GetSessionList();
            //$scope.WithServiceShipmentModal(data);
            toaster.pop({
                type: 'success',
                title: $scope.Frayte_Success,
                body: $scope.UploadSuccessfullMessage,
                showCloseButton: true
            });
            //intervaldata();
            //$scope.ShowInterval = true;
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        }
    };

    $scope.errorExcel = function (err) {
        AppSpinner.hideSpinnerTemplate();
        if (err.Message === "CSV file not valid, missing header name or may be wrong name") {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.Invalid_Excel_File_Missing_Required,
                showCloseButton: true
            });
            $scope.IsCollapsedval = false;
            $scope.GuideLinePopup();
        }
        else if (err.Message === "from country or to country or shipmentcurrency name is missing") {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.FromCountryToCountryNameMissing,
                showCloseButton: true
            });
            $scope.IsCollapsedval = false;
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                //body: err.Message,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        }


    };

    $scope.GetService = function (directBooking) {
        if (directBooking !== undefined) {
            $rootScope.GetServiceValue = ' ';
        }
        DbUploadShipmentService.GetErrorDetail(directBooking.ShipmentId, "DirectBooking_SS").then(function (response) {
            if (response.data) {
                directBooking = response.data[0];
                if (directBooking !== undefined &&
           directBooking !== null &&
           directBooking.ShipFrom !== undefined &&
           directBooking.ShipFrom !== null &&
           directBooking.ShipTo !== undefined &&
           directBooking.ShipTo !== null &&
           directBooking.ShipFrom.Country !== null &&
           directBooking.ShipTo.Country !== null &&
           directBooking.Package !== undefined &&
           directBooking.Package !== null) {
                    var weightTotal = $scope.PackagesTotal(directBooking.Package, 'Weight');
                    weightTotal = parseFloat(weightTotal);

                    if (directBooking.CustomerId === null || directBooking.CustomerId === 0) {
                        toaster.pop({
                            type: 'warning',
                            title: $scope.TitleFrayteWarning,
                            body: $scope.SelectCustomerValidation,
                            showCloseButton: true
                        });

                        return;
                    }
                    if (directBooking.CurrencyCode === null || directBooking.CurrencyCode === undefined) {
                        toaster.pop({
                            type: 'warning',
                            title: $scope.TitleFrayteWarning,
                            body: $scope.SelectCurrencyValidation,
                            showCloseButton: true
                        });

                        return;
                    }

                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'directBookingUploadShipment/directBookinGetService/directBookingGetService.tpl.html',
                        controller: 'DirectBookingGetServiceCtrl',
                        windowClass: 'DirectBookingService',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            directBookingObj: function () {
                                return directBooking;
                            },
                            IsRateShow: function () {
                                return $scope.IsRateShow;
                            },
                            CustomerDetail: function () {
                                return $scope.CustomerDetail;
                            },
                            AddressType: function () {
                                if ($scope.LogisticCompany !== undefined && $scope.LogisticCompany !== null && $scope.LogisticCompany !== '') {
                                    //if ($scope.LogisticCompany === 'Yodel') {
                                    return $scope.AddressType;
                                    //}
                                }
                                else {
                                    return '';
                                }
                            },
                            LogisticService: function () {
                                return "";
                            },
                            CallingFrom: function () {
                                return 'DirectBooking';
                            }
                        }
                    });

                    modalInstance.result.then(function (customerRateCard) {
                        $scope.SaveServiceCode(customerRateCard);
                    }, function () {

                    });
                }
                else {
                    //toaster.pop({
                    //    type: 'warning',
                    //    title: $scope.TitleFrayteWarning,
                    //    body: $scope.GetServiceValidation,
                    //    showCloseButton: true
                    //});
                }
            }
        }, function () {

        });

    };

    $scope.SaveServiceCode = function (CustomerRateCard) {
        DbUploadShipmentService.SaveServiceCode(CustomerRateCard.LogisticShipmentCode, CustomerRateCard.ShipmentId).then(function (response) {
            var Data = response.data;
            if (Data.Status) {
                toaster.pop({
                    type: 'success',
                    title: $scope.Frayte_Success,
                    body: $scope.ServiceCodeSavedSuccessfully,
                    showCloseButton: true
                });
                $scope.GetSessionShipments($scope.SessionRecord);
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.ErrorSavingServiceCode,
                showCloseButton: true
            });
        });
    };

    $scope.PackagesTotal = function (items, prop) {
        if (items === null || items === undefined) {
            return 0;
        }
        else {
            return items.reduce(function (a, b) {
                var convertB = 0.0;
                if (b[prop] !== undefined && b[prop] !== null) {
                    convertB = parseFloat(b[prop]);
                }
                var convertA = 0.0;
                if (a !== undefined && a !== null) {
                    convertA = parseFloat(a);
                }

                convertc = convertA + convertB;
                var f = convertc.toFixed(2);
                var swd = Number(parseFloat(convertc).toFixed(2)).toLocaleString('en', {
                    minimumFractionDigits: 2
                });

                return f;

            }, 0);
        }
    };

    $scope.ViewShipment = function (GridData) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBookingUploadShipment/directBookingViewShipments/directBookingViewShipments.tpl.html',
            controller: 'DirectBookingViewShipmentController',
            windowClass: 'DirectBookingService',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                SessionId: function () {
                    return GridData.SessionId;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };

    var filterUnsuccessfullShipment = function () {
        $scope.UnsuccessFullShipmentErrors = {
            Address: [],
            Package: [],
            Custom: [],
            Service: [],
            ServiceError: [],
            MiscErrors: [],
            Miscellaneous: []
        };


        $scope.UnsuccessFullShipments = [];
        $scope.SuccessFullShipments = [];
        $scope.UnsuccessFullWithoutShipments = [];

        angular.forEach($scope.ShipmentData, function (shipment, key) {
            //for(var shipment in  $scope.ShipmentData){
            if (shipment.Error !== null && shipment.Error.Status === false && $scope.ServiceType === 'ECOMMERCE_SS') {
                $scope.UnsuccessFullShipments.push(shipment);
                if (shipment.Error.Address != null && shipment.Error.Address.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Address.push(shipment.Error.Address);
                }
                if (shipment.Error.Package != null && shipment.Error.Package.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Package.push(shipment.Error.Package);
                }
                if (shipment.Error.Custom != null && shipment.Error.Custom.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Custom.push(shipment.Error.Custom);
                }
                if (shipment.Error.Service != null && shipment.Error.Service.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Service.push(shipment.Error.Service);
                }
                if (shipment.Error.ServiceError != null && shipment.Error.ServiceError.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.MiscErrors.push(shipment.Error.ServiceError);
                }
                if (shipment.Error.MiscErrors != null && shipment.Error.MiscErrors.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.push(shipment.Error.MiscErrors);
                }
                if (shipment.Error.Miscellaneous != null && shipment.Error.Miscellaneous.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Miscellaneous.push(shipment.Error.Miscellaneous);
                }
            }
            else if (shipment.Errors !== null && shipment.Errors.length === 0 && $scope.ServiceType === 'ECOMMERCE_WS') {
                shipment.BookingStatusType = 'Successfull';
                $scope.SuccessFullShipments.push(shipment);
            }
            else if (shipment.Errors !== null && shipment.Errors.length > 0 && $scope.ServiceType === 'ECOMMERCE_WS') {
                $scope.UnsuccessFullShipments.push(shipment);
            }
        });
        $scope.ShipmentDataLength = $scope.ShipmentData.length - $scope.UnsuccessFullShipments.length;

        if ($scope.ShipmentDataLength > 1) {
            $scope.shipment = $scope.ShipmentAre;
        }                     
        else {
            $scope.shipment = $scope.ShipmentIs;
        }
    };

    $scope.WithServiceShipmentModal = function (shipments) {

        //$scope.UnsuccessFullShipments = [];
        //$scope.ShipmentData = shipments;
        //$scope.ErrorList = shipments;
        //$scope.ServiceType = "DirectBooking_SS";
        //filterUnsuccessfullShipment();
        //toaster.pop({
        //    type: 'error',
        //    title: $scope.Frayte_Error,
        //    body: 'bind-unsafe-html',
        //    showCloseButton: true
        //});
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithServiceMessage/uploadShipmentWithServiceMessage.tpl.html',
            controller: 'WithServiceMessageController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ShipmentData: function () {
                    return shipments;
                },
                ServiceType: function () {
                    return $scope.ServiceType;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };

    $scope.WithServiceFinalMessageShipmentModal = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithServiceFinalMessage/uploadShipmentWithServiceFinalMessage.tpl.html',
            controller: 'WithServiceFinalMessageController',
            windowClass: '',
            size: 'lg',
            resolve: {
                BatchProcessedShipments: function () {
                    return $scope.BatchProcessedShipments;
                },
                BatchUnprocessedShipments: function () {
                    return $scope.BatchUnprocessedShipments;
                },
                BatchProcess: function () {
                    return $scope.BatchProcess;
                },
                SessionRecord: function () {
                    return $scope.SessionRecord;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };

    $scope.GuideLinePopup = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBookingUploadShipment/directBookingGuideLine/directBookingGuideLine.tpl.html',
            controller: 'GuideLineController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                IsCollapeFillValue: function () {
                    return $scope.IsCollapsedval;
                }
            }
        });
        modalInstance.result.then(function () {
        }, function () {
        });
    };

    $scope.SaveUploadShipmentWithService = function (gridDirectBooking) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBookingUploadShipment/directBookingForm/directBookingForm.tpl.html',
            controller: 'DirectBookingFormController',
            windowClass: 'DirectBookingService',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ShipmentId: function () {
                    return gridDirectBooking.ShipmentId;
                },
                TrackingNo: function () {
                    return gridDirectBooking.TrackingNo;
                },
                CourierCompany: function () {
                    return gridDirectBooking.DisplayName;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.WithServiceShipments();
        }, function () {
        });
    };

    $scope.PrintAllLabel = function (Session) {
        DbUploadShipmentService.SaveIsSessionPrint(Session.SessionId).then(function (response) {
            var result = response.data;
            if (result) {
                toaster.pop({
                    type: 'success',
                    allowHtml: true,
                    title: $scope.Frayte_Success,
                    body: $scope.ShipmentLabelPrintedIn30Minutes,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.ErrorPrintingLabels,
                showCloseButton: true
            });
        });
    };

    var errorShowWithServiceReturn = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithServiceError/uploadShipmentWithServiceReturnErrors.tpl.html',
            controller: 'WithServiceReturnErrorsController',
            windowClass: '',
            size: 'lg',
            resolve: {
                ShipmentData: function () {
                    return row;
                },
                ServiceType: function () {
                    return $scope.ServiceType;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };

    var intervaldata = function () {
        var i = 1;
        $scope.stop = $interval(function () {

            DbUploadShipmentService.GetUpdatedBatchProcess($scope.CustomerId, $scope.SessionRecord.SessionId).then(function (response) {
                $scope.BatchProcess = response.data.TotalShipments;
                $scope.BatchProcessedShipments = response.data.ProcessedShipment;
                $scope.BatchUnprocessedShipments = response.data.UnprocessedShipment;
                $scope.Batchpercentage = (($scope.BatchProcessedShipments / $scope.BatchProcess) * 100).toFixed(2);
                if ($scope.BatchProcessedShipments > 0) {
                    AppSpinner.hideSpinnerTemplate();
                }
                if (i === 1) {
                    //$scope.WithServiceShipments();
                    $scope.GetSessionList();
                    $scope.GetSessionShipments($scope.SessionRecord);
                }
                i++;
                var total = $scope.BatchUnprocessedShipments + $scope.BatchProcessedShipments;
                if ($scope.BatchProcess === total) {
                    AppSpinner.hideSpinnerTemplate();
                    //RemoveShipmentWithServiceUnsuccessfullShipment$timeout(function () {
                    $scope.stopFight();
                    $scope.ShowInterval = false;
                    $scope.PlaceBookingflag = false;
                    //$scope.WithServiceFinalMessageShipmentModal();
                    toaster.pop({
                        type: 'success',
                        allowHtml: true,
                        title: $scope.Frayte_Success,
                        //body: '<div>' +  'Total Shipments ' + '=' + total + '<br>' + 'Processed Shipments ' + '=' + $scope.BatchProcessedShipments + '<br>'  + 'UnProcessed Shipments ' + '=' + $scope.BatchUnprocessedShipments + '</div>',
                        body: $scope.BatchProcessedShipments + ' Out of ' + total + ' Shipment Created Successfully,' + ' for ' + $scope.SessionRecord.SessionName + ' for more information go to track and trace.',
                        //body: 'bind-unsafe-html',
                        //bodyOutputType: 'directive',
                        showCloseButton: true
                    });
                    //$scope.WithServiceShipments();
                    //$scope.GetSessionShipments($scope.SessionRecord);
                    $scope.GetSessionList();
                    //}, 3000);
                }
            });
        }, 3000);

    };

    $scope.stopFight = function () {
        if (angular.isDefined($scope.stop)) {
            $interval.cancel($scope.stop);
            $scope.stop = undefined;
        }
    };

    $scope.CorrectError = function (row) {
        DbUploadShipmentService.GetErrorDetail(row.ShipmentId, "DirectBooking_SS").then(function (response) {
            if (response.data[0]) {
                var flag = false;
                if (response.data[0].Errors.length > 0) {
                    for (i = 0; i < response.data[0].Errors.length; i++) {
                        if (response.data[0].Errors[i].includes("CartonValue is empty") ||
                            response.data[0].Errors[i].includes("Length is empty") ||
                            response.data[0].Errors[i].includes("Width is empty") ||
                            response.data[0].Errors[i].includes("Height is empty") ||
                            response.data[0].Errors[i].includes("Weight is empty") ||
                            response.data[0].Errors[i].includes("DeclaredValue is empty") ||
                            response.data[0].Errors[i].includes("ShipmentContents is empty")) {
                            flag = true;
                        }
                    }
                    if (flag) {
                        $scope.AddnewShipment(row.ShipmentId);
                    }
                    else {
                        $scope.ErrorShowWithService(row);
                    }
                }
                //else if (response.data.Errors) {
                //    $scope.ErrorShowWithService(row);
                //}
            }
        });
    };

    $scope.ErrorShowWithService = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithoutService/uploadShipmentWithoutServiceErrors.tpl.html',
            controller: 'DirectBookingServiceErrorsController',
            windowClass: '',
            backdrop: true,
            size: 'md',
            resolve: {
                ShipmentData: function () {
                    return row;
                }
            }
        });
        modalInstance.result.then(function () {
            //$scope.WithServiceShipments();
            $scope.GetSessionShipments($scope.SessionRecord);
        }, function () {
        });
    };

    $scope.ErrorShowWithServiceSuccessfulShipment = function (row) {
        if (row.IsEasyPostError) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'directBookingUploadShipment/uploadShipmentWithServiceSuccessfulError/uploadShipmentWithServiceSuccessfulErrors.tpl.html',
                controller: 'GetServiceErrorsController',
                windowClass: '',
                size: 'lg',
                backdrop: true,
                resolve: {
                    ShipmentData: function () {
                        return row;
                    }
                }
            });
            modalInstance.result.then(function () {

            }, function () {
            });
        }
        //else if (row.errors.length > 0) {

        //}
    };

    $scope.GetMouseEnter = function (row) {
        if (row.IsEasyPostError) {
            row.ShowText = 'Double click to view error(s).';
            return 'mouseenter';
        }
        else {
            row.ShowText = '';
            return;
        }
    };

    $scope.DownLoadUnsuccessfulshipmentswithservice = function () {
        if ($scope.ShipmentsWithServiceUnsuccessful.length > 0) {
            var modalOptions = {
                headerText: $scope.Download_Shipments,
                bodyText: $scope.Reupload_Place_Booking
            };

            ModalService.Confirm({}, modalOptions).then(function (result) {
                DbUploadShipmentService.GenerateUnsucessfulShipmentsWithServcie($scope.ShipmentsWithServiceUnsuccessful).then(function (response) {
                    $scope.RemoveShipmentCount2 = 0;
                    if (response.data !== null) {
                        var fileInfo = response.data;
                        var fileName = {
                            FileName: response.data.FileName,
                            FilePath: response.data.FilePath
                        };

                        $http({
                            method: 'POST',
                            url: config.SERVICE_URL + '/eCommerceUploadShipment/DownloadUnsucessfulShipments',
                            data: fileName,
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
                                    //AppSpinner.hideSpinnerTemplate();
                                    toaster.pop({
                                        type: 'success',
                                        title: $scope.Frayte_Success,
                                        body: $scope.DownloadSuccessfullCsv,
                                        showCloseButton: true
                                    });
                                } catch (ex) {
                                    $scope.RemoveShipmentCount2 = 0;
                                    //AppSpinner.hideSpinnerTemplate();
                                    $scope.WithServiceShipments();
                                    $scope.ShipmentsWithServiceUnsuccessful = [];
                                    $window.open(fileInfo.FilePath, "_blank");
                                    console.log(ex);
                                }

                            }
                        })
                       .error(function (data) {
                           // AppSpinner.hideSpinnerTemplate();
                           console.log(data);
                           toaster.pop({
                               type: 'warning',
                               title: $scope.FrayteWarningValidation,
                               body: $scope.ShipmentNotAvaliable,
                               showCloseButton: true
                           });
                       });

                    }
                    else {
                        //AppSpinner.hideSpinnerTemplate();
                        toaster.pop({
                            type: 'warning',
                            title: $scope.FrayteWarningValidation,
                            body: $scope.ShipmentNotAvaliable,
                            showCloseButton: true
                        });
                    }

                }, function () {
                    //AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarningValidation,
                        body: $scope.ShipmentNotAvaliable,
                        showCloseButton: true
                    });
                });
            });
        }
    };

    $scope.GetServices = function () {
        //   DbUploadShipmentService.GetServices().then(function (response) {
        //       if (response.data.length > 0) {
        //           $scope.ServcieDetail = response.data;
        //       }
        //       else {
        //           console.log(response);
        //       }
        //   },
        //function () {
        //    if (response.status !== 401) {
        //        toaster.pop({
        //            type: 'error',
        //            title: $scope.TitleFrayteError,
        //            body: $scope.TextErrorOccuredDuringUpload,
        //            showCloseButton: true
        //        });
        //    }
        //});

        //DbUploadShipmentService.GetServiceCode().then(function (response) {
        //    if (response.data.length > 0) {
        //        $scope.ServcieCodeDetail = response.data;
        //    }
        //});
        DbUploadShipmentService.GetLogisticServiceCode($scope.OperationZoneId, $scope.CustomerId).then(function (response) {
            if (response.data.length > 0) {
                $scope.ServcieCodeDetail = response.data;
            }
        });
        //$scope.ServcieCodeDetail = [{
        //    ServiceCodeDescription: 'DHL',
        //    ServiceCode: 'D0012'
        //},
        //{
        //    ServiceCodeDescription: 'Ukmail',
        //    ServiceCode: 'D0013'
        //},
        //{
        //    ServiceCodeDescription: 'Yodel',
        //    ServiceCode: 'D0014'
        //},
        //{
        //    ServiceCodeDescription: 'Hermes',
        //    ServiceCode: 'D0015'
        //}];

    };

    var checkShipmentsValidation = function () {
        $scope.flag = false;

        for (i = 0; i < $scope.gridOptionsdata.length; i++) {

            if ($scope.gridOptionsdata[i].IsSuccessFull === true) {
                $scope.flag = true;
                return;
            }
        }
    };

    $scope.SaveShipmentWithService = function () {

        $scope.CheckValidation = checkShipmentsValidation($scope.gridOptionsdata);

        if ($scope.flag === false && $scope.gridOptionsdata.length > 0) {
            $scope.PlaceBookingflag = true;
            AppSpinner.showSpinnerTemplate($scope.UploadingDirectBookingShipments, $scope.Template);
            DbUploadShipmentService.SaveShipmentWithService($scope.gridOptionsdata).then(function (response) {
                //if (response.status === 200 && response.data[0].Error !== null && response.data[0].Error.Status === true) {
                if (response.status === 200) {
                    var res = response.data;
                    $scope.GetSessionShipments($scope.SessionRecord);
                    $scope.GetSessionList();
                    //$scope.WithServiceShipments();
                    //errorShowWithServiceReturn(res);
                    //$scope.RemoveShipmentCount1 = 0;
                    //$scope.WithServiceShipments();
                    intervaldata();
                    $scope.ShowInterval = true;
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorOccuredDuringUpload,
                        showCloseButton: true
                    });
                }
            },
        function () {

            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.MissingInformationInSomeSessionShipment,
                showCloseButton: true
            });
        }
    };

    var remainedFieldsPopup = function (DirectBooking) {
        $scope.RemainField = [];
        var Shipment = DirectBooking;

        for (i = 0; i < Shipment.Packages.length; i++) {
            if (Shipment.ShipFrom.Country !== null && Shipment.ShipFrom.Country !== undefined && Shipment.ShipTo.Country !== null && Shipment.ShipTo.Country !== null && Shipment.ShipFrom.Country.Code === Shipment.ShipTo.Country.Code) {
                if ((Shipment.Packages[i].CartoonValue === 0 || Shipment.Packages[i].CartoonValue === null || Shipment.Packages[i].CartoonValue === undefined) ||
                (Shipment.Packages[i].Length === 0 || Shipment.Packages[i].Length === null || Shipment.Packages[i].Length === undefined) ||
                (Shipment.Packages[i].Width === 0 || Shipment.Packages[i].Width === null || Shipment.Packages[i].Width === undefined) ||
                (Shipment.Packages[i].Height === 0 || Shipment.Packages[i].Height === null || Shipment.Packages[i].Height === undefined) ||
                (Shipment.Packages[i].Weight === 0 || Shipment.Packages[i].Weight === null || Shipment.Packages[i].Weight === undefined) ||
                (Shipment.Packages[i].Content === "" || Shipment.Packages[i].Content === null || Shipment.Packages[i].Content === undefined)) {
                    $scope.ErrorFormShow = true;
                    break;
                }
                else {
                    $scope.ErrorFormShow = false;
                }
            }
            else {
                if ((Shipment.Packages[i].CartoonValue === 0 || Shipment.Packages[i].CartoonValue === null || Shipment.Packages[i].CartoonValue === undefined) ||
                (Shipment.Packages[i].Length === 0 || Shipment.Packages[i].Length === null || Shipment.Packages[i].Length === undefined) ||
                (Shipment.Packages[i].Width === 0 || Shipment.Packages[i].Width === null || Shipment.Packages[i].Width === undefined) ||
                (Shipment.Packages[i].Height === 0 || Shipment.Packages[i].Height === null || Shipment.Packages[i].Height === undefined) ||
                (Shipment.Packages[i].Weight === 0 || Shipment.Packages[i].Weight === null || Shipment.Packages[i].Weight === undefined) ||
                (Shipment.Packages[i].Value === 0 || Shipment.Packages[i].Value === null || Shipment.Packages[i].Value === undefined) ||
                (Shipment.Packages[i].Content === "" || Shipment.Packages[i].Content === null || Shipment.Packages[i].Content === undefined)) {
                    $scope.ErrorFormShow = true;
                    break;
                }
                else {
                    $scope.ErrorFormShow = false;
                }
            }
        }

        if ($scope.ButtonValue === "GetServices") {
            if (Shipment.ShipFrom.Country === "" || Shipment.ShipFrom.Country === null || Shipment.ShipFrom.Country === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "Country";
                $scope.RemainedFields.FieldLabel = "From Country";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.TrackBYOBJ = "ParcelType.CountryId";
                $scope.RemainedFields.IterationFor1 = "ParcelType.Name";
                $scope.RemainedFields.IterationForAs = "ParcelType";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "fromCountry";
                $scope.RemainedFields.RequiredMessage = "CountryValidationError";
                $scope.RemainedFields.GeneralObj = $scope.CountriesRepo;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.ShipFrom.PostCode === "" || Shipment.ShipFrom.PostCode === null || Shipment.ShipFrom.PostCode === undefined) && ((Shipment.ShipFrom.Country !== null && Shipment.ShipFrom.Country !== "" && Shipment.ShipFrom.Country !== undefined) && Shipment.ShipFrom.Country.Code !== "HKG")) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "PostCode";
                $scope.RemainedFields.FieldLabel = "From FromPostCode";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromPostCodeLabel";
                $scope.RemainedFields.InputTypeName = "fromPostCode";
                $scope.RemainedFields.RequiredMessage = "PostalCodeValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipFrom.FirstName === "" || Shipment.ShipFrom.FirstName === null || Shipment.ShipFrom.FirstName === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "FirstName";
                $scope.RemainedFields.FieldLabel = "From Contact First Name";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromFirstNameLabel";
                $scope.RemainedFields.InputTypeName = "fromFirstName";
                $scope.RemainedFields.RequiredMessage = "FirstName_Required";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipFrom.LastName === "" || Shipment.ShipFrom.LastName === null || Shipment.ShipFrom.LastName === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "LastName";
                $scope.RemainedFields.FieldLabel = "From Contact Last Name";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromLastNameLabel";
                $scope.RemainedFields.InputTypeName = "fromLastName";
                $scope.RemainedFields.RequiredMessage = "LastName_Required";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipFrom.Address === "" || Shipment.ShipFrom.Address === null || Shipment.ShipFrom.Address === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "From Address 1";
                $scope.RemainedFields.FieldName = "Address";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromAddressNameLabel";
                $scope.RemainedFields.InputTypeName = "fromAddress";
                $scope.RemainedFields.RequiredMessage = "AddressValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipFrom.City === "" || Shipment.ShipFrom.City === null || Shipment.ShipFrom.City === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "From City";
                $scope.RemainedFields.FieldName = "City";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromCityNameLabel";
                $scope.RemainedFields.InputTypeName = "fromCity";
                $scope.RemainedFields.RequiredMessage = "CityValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.ShipFrom.State === "" || Shipment.ShipFrom.State === null || Shipment.ShipFrom.State === undefined) && (Shipment.ShipFrom.Country !== undefined && Shipment.ShipFrom.Country !== null && Shipment.ShipFrom.Country.Code !== 'HKG' && Shipment.ShipFrom.Country.Code !== 'GBR')) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "From State";
                $scope.RemainedFields.FieldName = "State";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromStateNameLabel";
                $scope.RemainedFields.InputTypeName = "fromState";
                $scope.RemainedFields.RequiredMessage = "StateValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipFrom.Phone === "" || Shipment.ShipFrom.Phone === null || Shipment.ShipFrom.Phone === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "From TelephoneNo";
                $scope.RemainedFields.FieldName = "Phone";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromPhoneLabel";
                $scope.RemainedFields.InputTypeName = "fromPhone";
                $scope.RemainedFields.RequiredMessage = "TelephoneValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.Country === "" || Shipment.ShipTo.Country === null || Shipment.ShipTo.Country === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "Country";
                $scope.RemainedFields.FieldLabel = "To Country";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.TrackBYOBJ = "ParcelType.CountryId";
                $scope.RemainedFields.IterationFor1 = "ParcelType.Name";
                $scope.RemainedFields.IterationForAs = "ParcelType";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "toCountry";
                $scope.RemainedFields.RequiredMessage = "CountryValidationError";
                $scope.RemainedFields.GeneralObj = $scope.CountriesRepo;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.ShipTo.PostCode === "" || Shipment.ShipTo.PostCode === null || Shipment.ShipTo.PostCode === undefined) &&
                (Shipment.ShipTo.Country !== "" && Shipment.ShipTo.Country !== null && Shipment.ShipTo.Country !== undefined && Shipment.ShipTo.Country.Code !== "HKG")) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "PostCode";
                $scope.RemainedFields.FieldLabel = "To PostCode";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toPostCodeLabel";
                $scope.RemainedFields.InputTypeName = "toPostCode";
                $scope.RemainedFields.RequiredMessage = "PostalCodeValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.FirstName === "" || Shipment.ShipTo.FirstName === null || Shipment.ShipTo.FirstName === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "FirstName";
                $scope.RemainedFields.FieldLabel = "To Contact First Name";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toFirstNameLabel";
                $scope.RemainedFields.InputTypeName = "toFirstName";
                $scope.RemainedFields.RequiredMessage = "FirstName_Required";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.LastName === "" || Shipment.ShipTo.LastName === null || Shipment.ShipTo.LastName === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "LastName";
                $scope.RemainedFields.FieldLabel = "To Contact Last Name";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toLastNameLabel";
                $scope.RemainedFields.InputTypeName = "toLastName";
                $scope.RemainedFields.RequiredMessage = "LastName_Required";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.Address === "" || Shipment.ShipTo.Address === null || Shipment.ShipTo.Address === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "To Address 1";
                $scope.RemainedFields.FieldName = "Address";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toAddressNameLabel";
                $scope.RemainedFields.InputTypeName = "toAddress";
                $scope.RemainedFields.RequiredMessage = "AddressValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.City === "" || Shipment.ShipTo.City === null || Shipment.ShipTo.City === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "To City";
                $scope.RemainedFields.FieldName = "City";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toCityNameLabel";
                $scope.RemainedFields.InputTypeName = "toCity";
                $scope.RemainedFields.RequiredMessage = "CityValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.ShipTo.State === "" || Shipment.ShipTo.State === null || Shipment.ShipTo.State === undefined) && (Shipment.ShipTo.Country !== undefined && Shipment.ShipTo.Country !== null && Shipment.ShipTo.Country.Code !== 'HKG' && Shipment.ShipTo.Country.Code !== 'GBR')) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "To State";
                $scope.RemainedFields.FieldName = "State";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toStateNameLabel";
                $scope.RemainedFields.InputTypeName = "toState";
                $scope.RemainedFields.RequiredMessage = "StateValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.Phone === "" || Shipment.ShipTo.Phone === null || Shipment.ShipTo.Phone === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "To TelephoneNo";
                $scope.RemainedFields.FieldName = "Phone";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toPhoneLabel";
                $scope.RemainedFields.InputTypeName = "toPhone";
                $scope.RemainedFields.RequiredMessage = "TelephoneValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.PayTaxAndDuties === "" || Shipment.PayTaxAndDuties === null || Shipment.PayTaxAndDuties === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Pay Tax And Duties";
                $scope.RemainedFields.FieldName = "PayTaxAndDuties";
                $scope.RemainedFields.FileType = "radio";
                $scope.RemainedFields.ShowDiv = "MainObjProperty";
                $scope.RemainedFields.LabelName = "payTaxAndDutiesLabel";
                $scope.RemainedFields.InputTypeName = "payTaxAndDuties";
                $scope.RemainedFields.RequiredMessage = "PayTaxAndDuties is required.";
                $scope.RemainedFields.RadioButtonValues = ["Shipper", "Receiver", "ThirdParty"];
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.PakageCalculatonType === "" || Shipment.PakageCalculatonType === null || Shipment.PakageCalculatonType === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Package Calculation Type";
                $scope.RemainedFields.FieldName = "PakageCalculatonType";
                $scope.RemainedFields.FileType = "radio";
                $scope.RemainedFields.InputTypeName = "pakageCalculatonType";
                $scope.RemainedFields.ShowDiv = "MainObjProperty";
                $scope.RemainedFields.LabelName = "pakageCalculatonTypeLabel";
                $scope.RemainedFields.RequiredMessage = "PackageCalculatonType is required.";
                $scope.RemainedFields.RadioButtonValues = ["kgToCms", "lbToInchs"];
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ParcelType === "" || Shipment.ParcelType === null || Shipment.ParcelType === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Parcel Type";
                $scope.RemainedFields.FieldName = "ParcelType";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.DashString = "";
                $scope.RemainedFields.TrackBYOBJ = "ParcelType.ParcelType";
                $scope.RemainedFields.IterationFor1 = "ParcelType.ParcelDescription";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "parcelType";
                $scope.RemainedFields.LabelName = "parcelTypeLabel";
                $scope.RemainedFields.IterationForAs = "ParcelType.ParcelType";
                $scope.RemainedFields.RequiredMessage = "ParcelType_required";
                $scope.RemainedFields.ShowDiv = "MainObjProperty";
                $scope.RemainedFields.GeneralObj = $scope.ParcelTypes;

                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.Currency === "" || Shipment.Currency === null || Shipment.Currency === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Currency";
                $scope.RemainedFields.FieldName = "Currency";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.DashString = "+ ' - ' +";
                $scope.RemainedFields.TrackBYOBJ = "ParcelType.CurrencyCode";
                $scope.RemainedFields.IterationFor1 = "ParcelType.CurrencyCode";
                $scope.RemainedFields.IterationFor2 = "ParcelType.CurrencyDescription";
                $scope.RemainedFields.InputTypeName = "currency";
                $scope.RemainedFields.LabelName = "currencyLabel";
                $scope.RemainedFields.ShowDiv = "MainObjProperty";
                $scope.RemainedFields.IterationForAs = "ParcelType.CurrencyCode";
                $scope.RemainedFields.RequiredMessage = "Currency_required";
                $scope.RemainedFields.GeneralObj = $scope.CurrencyTypes;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ReferenceDetail.Reference1 === "" || Shipment.ReferenceDetail.Reference1 === null || Shipment.ReferenceDetail.Reference1 === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Shipment Reference";
                $scope.RemainedFields.FieldName = "Reference1";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.LabelName = "courierCompanyLable";
                $scope.RemainedFields.ShowDiv = "ReferenceDetail";
                $scope.RemainedFields.InputTypeName = "courierCompany";
                $scope.RemainedFields.RequiredMessage = "Reference_required";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.AddressType === "" || Shipment.AddressType === null || Shipment.AddressType === undefined) &&
                $scope.LogisticCompany === 'Yodel' && Shipment.ShipFrom.Country.Code === 'GBR' && Shipment.ShipTo.Country.Code === 'GBR') {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Address Type";
                $scope.RemainedFields.FieldName = "AddressType";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.LabelName = "addressTypeLable";
                $scope.RemainedFields.ShowDiv = "MainObjProperty";
                $scope.RemainedFields.InputTypeName = "addressType";
                $scope.RemainedFields.RequiredMessage = "Address_Type";
                $scope.RemainField.push($scope.RemainedFields);
            }
            //if ($scope.directBooking.CustomerRateCard != null && $scope.directBooking.CustomerRateCard.LogisticServiceId > 0 && Shipment.ReferenceDetail !== null && Shipment.ReferenceDetail !== undefined) {
            //    if (Shipment.ReferenceDetail.CollectionDate === undefined || Shipment.ReferenceDetail.CollectionDate === null || Shipment.ReferenceDetail.CollectionDate === "" ) {
            //        remainFieldJson();
            //        $scope.RemainedFields.FieldLabel = "Collection Date";
            //        $scope.RemainedFields.FieldName = "CollectionDate";
            //        $scope.RemainedFields.FileType = "Date";
            //        $scope.RemainedFields.LabelName = "collectionDateLable";
            //        $scope.RemainedFields.ShowDiv = "DirectBooking";
            //        $scope.RemainedFields.InputTypeName = "collectionDate";
            //        $scope.RemainedFields.RequiredMessage = "Collection Date is required.";
            //        $scope.RemainField.push($scope.RemainedFields);
            //    }
            //    if (Shipment.ReferenceDetail.CollectionTime === undefined || Shipment.ReferenceDetail.CollectionTime === null || Shipment.ReferenceDetail.CollectionTime ==="") {
            //        remainFieldJson();
            //        $scope.RemainedFields.FieldLabel = "Collection Time";
            //        $scope.RemainedFields.FieldName = "CollectionTime";
            //        $scope.RemainedFields.FileType = "DirectBooking";
            //        $scope.RemainedFields.LabelName = "collectionTimeLable";
            //        $scope.RemainedFields.ShowDiv = "ReferenceDetail";
            //        $scope.RemainedFields.InputTypeName = "collectionTime";
            //        $scope.RemainedFields.RequiredMessage = "Collection Time is required.";
            //        $scope.RemainField.push($scope.RemainedFields);
            //    }
            //}
        }

        if (Shipment.ShipFrom.Country != null && Shipment.ShipTo.Country != null && Shipment.ShipFrom.Country.Code != Shipment.ShipTo.Country.Code && $scope.ButtonValue === "PlaceBooking") {
            if (Shipment.CustomInfo.ContentsType === "" || Shipment.CustomInfo.ContentsType === null || Shipment.CustomInfo.ContentsType === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Content_Type";
                $scope.RemainedFields.FieldName = "ContentsType";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.DashString = "";
                $scope.RemainedFields.TrackBYOBJ = "";
                $scope.RemainedFields.IterationFor1 = "ParcelType.name";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "contentsType";
                $scope.RemainedFields.LabelName = "contentsTypeLabel";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.IterationForAs = "ParcelType.value";
                $scope.RemainedFields.RequiredMessage = "contentsTypeValidError";
                $scope.RemainedFields.GeneralObj = $scope.ContentsType;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.CustomInfo.RestrictionType === "" || Shipment.CustomInfo.RestrictionType === null || Shipment.CustomInfo.RestrictionType === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Restriction_Type";
                $scope.RemainedFields.FieldName = "RestrictionType";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.DashString = "";
                $scope.RemainedFields.TrackBYOBJ = "";
                $scope.RemainedFields.IterationFor1 = "ParcelType.name";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "restrictionType";
                $scope.RemainedFields.LabelName = "restrictionTypeLabel";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.IterationForAs = "ParcelType.value";
                $scope.RemainedFields.RequiredMessage = "restrictionTypeValidError";
                $scope.RemainedFields.GeneralObj = $scope.RestrictionType;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.CustomInfo.RestrictionComments === "" || Shipment.CustomInfo.RestrictionComments === null || Shipment.CustomInfo.RestrictionComments === undefined) &&
                (Shipment.CustomInfo.RestrictionType !== 'quarantine' && Shipment.CustomInfo.RestrictionType !== 'sanitary_phytosanitary_inspection')) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Restriction_Explanation";
                $scope.RemainedFields.FieldName = "RestrictionComments";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.LabelName = "restrictionCommentsLable";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.InputTypeName = "restrictionComments";
                $scope.RemainedFields.RequiredMessage = "restrictionCommentsValidError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.CustomInfo.NonDeliveryOption === "" || Shipment.CustomInfo.NonDeliveryOption === null || Shipment.CustomInfo.NonDeliveryOption === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "nonDeliveryOption";
                $scope.RemainedFields.FieldName = "NonDeliveryOption";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.DashString = "";
                $scope.RemainedFields.TrackBYOBJ = "";
                $scope.RemainedFields.IterationFor1 = "ParcelType.name";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "nonDeliveryOption";
                $scope.RemainedFields.LabelName = "nonDeliveryOptionLabel";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.IterationForAs = "ParcelType.value";
                $scope.RemainedFields.RequiredMessage = "nonDeliveryOptionValidError";
                $scope.RemainedFields.GeneralObj = $scope.NonDeliveryOption;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.CustomInfo.CustomsSigner === "" || Shipment.CustomInfo.CustomsSigner === null || Shipment.CustomInfo.CustomsSigner === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "customsSigner";
                $scope.RemainedFields.FieldName = "CustomsSigner";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.LabelName = "customsSignerLable";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.InputTypeName = "customsSigner";
                $scope.RemainedFields.RequiredMessage = "customsSignerValidError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.CustomInfo.CustomsCertify === "" || Shipment.CustomInfo.CustomsCertify === null || Shipment.CustomInfo.CustomsCertify === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "By_filling_validation";
                $scope.RemainedFields.FieldName = "CustomsCertify";
                $scope.RemainedFields.FileType = "checkbox";
                $scope.RemainedFields.LabelName = "customsCertifyLable";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.InputTypeName = "customsCertify";
                $scope.RemainedFields.RequiredMessage = "";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.CustomerRateCard != null && (Shipment.CustomerRateCard.CourierName === "UKMail" || Shipment.CustomerRateCard.CourierName === "Yodel" || Shipment.CustomerRateCard.CourierName === "Hermes")) {
                if (Shipment.CustomInfo.CatagoryOfItem === "" || Shipment.CustomInfo.CatagoryOfItem === null || Shipment.CustomInfo.CatagoryOfItem === undefined) {
                    remainFieldJson();
                    $scope.RemainedFields.FieldLabel = "Category_Item";
                    $scope.RemainedFields.FieldName = "CatagoryOfItem";
                    $scope.RemainedFields.FileType = "text";
                    $scope.RemainedFields.LabelName = "catagoryOfItemLable";
                    $scope.RemainedFields.ShowDiv = "CustomInfo";
                    $scope.RemainedFields.InputTypeName = "catagoryOfItem";
                    $scope.RemainedFields.RequiredMessage = "Category_Item_required";
                    $scope.RemainField.push($scope.RemainedFields);
                }
                if (Shipment.CustomInfo.CatagoryOfItemExplanation === "" || Shipment.CustomInfo.CatagoryOfItemExplanation === null || Shipment.CustomInfo.CatagoryOfItemExplanation === undefined) {
                    remainFieldJson();
                    $scope.RemainedFields.FieldLabel = "Category_Explanation";
                    $scope.RemainedFields.FieldName = "CatagoryOfItemExplanation";
                    $scope.RemainedFields.FileType = "text";
                    $scope.RemainedFields.LabelName = "catagoryOfItemExplanationLable";
                    $scope.RemainedFields.ShowDiv = "CustomInfo";
                    $scope.RemainedFields.InputTypeName = "catagoryOfItemExplanation";
                    $scope.RemainedFields.RequiredMessage = "Content_Explanation_Required";
                    $scope.RemainField.push($scope.RemainedFields);
                }

            }

        }

        return $scope.RemainField;
    };

    $scope.RemoveShipmentWithService = function () {
        if ($scope.RemoveShipmentCount1 > 0) {
            AppSpinner.showSpinnerTemplate($scope.RemovingShipments, $scope.Template);
            DbUploadShipmentService.RemoveShipmentWithService($scope.ShipmentsWithService).then(function (response) {
                //if (response.status === 200 && response.data[0].Error !== null && response.data[0].Error.Status === true) {
                AppSpinner.hideSpinnerTemplate();
                if (response.status === 200) {
                    var res = response.data;
                    $scope.IsSelected = false;
                    $scope.WithServiceShipments();
                    $scope.RemoveShipmentCount1 = 0;
                    //errorShowWithServiceReturn(res);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.ShipmentDeletedFromSession,
                        showCloseButton: true
                    });
                    //$scope.WithServiceShipments();


                }
                else {
                    $scope.RemoveShipmentCount1 = 0;
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorOccuredDuringUpload,
                        showCloseButton: true
                    });
                }
            },
        function () {
            $scope.RemoveShipmentCount = 0;
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.selectatleastone,
                showCloseButton: true
            });
        }
    };

    $scope.RemoveShipmentWithServiceUnsuccessfullShipment = function () {
        if ($scope.RemoveShipmentCount2 > 0) {
            AppSpinner.showSpinnerTemplate($scope.RemovingShipments, $scope.Template);
            DbUploadShipmentService.RemoveShipmentWithService($scope.ShipmentsWithServiceUnsuccessful).then(function (response) {
                //if (response.status === 200 && response.data[0].Error !== null && response.data[0].Error.Status === true) {
                AppSpinner.hideSpinnerTemplate();
                if (response.status === 200) {
                    var res = response.data;
                    $scope.RemoveShipmentCount2 = 0;
                    $scope.IsSelected = false;
                    $scope.ShipmentsWithServiceUnsuccessful = [];
                    $scope.WithServiceShipments();
                    //errorShowWithServiceReturn(res);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.ShipmentDeletedFromSession,
                        showCloseButton: true
                    });
                    //$scope.WithServiceShipments();


                }
                else {
                    $scope.RemoveShipmentCount2 = 0;
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorOccuredDuringUpload,
                        showCloseButton: true
                    });
                }
            },
        function () {
            $scope.RemoveShipmentCount2 = 0;
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.selectatleastone,
                showCloseButton: true
            });
        }
    };

    $scope.SetServcieDetail = function (Service) {
        $scope.ServiceCount = 0;
        for (i = 0; i < $scope.gridOptionsSuccessfuldata.length; i++) {

            if ($scope.gridOptionsSuccessfuldata[i].ServiceValue !== null && $scope.gridOptionsSuccessfuldata[i].ServiceValue !== undefined && $scope.gridOptionsSuccessfuldata[i].ServiceValue === true) {
                $scope.ServiceCount = $scope.ServiceCount + 1;
                $scope.gridOptionsSuccessfuldata[i].Service = {};
                $scope.gridOptionsSuccessfuldata[i].Service = Service;

            }

            else {

                $scope.gridOptionsSuccessfuldata[i].ServiceValue = false;
                $scope.gridOptionsSuccessfuldata[i].Service = {};
            }
        }

    };

    $scope.WithServiceShipments = function () {
        AppSpinner.showSpinnerTemplate($scope.WithServiceShipments, $scope.Template);
        DbUploadShipmentService.GetShipmentsFromDraft($scope.CustomerId).then(function (response) {
            $scope.gridOptionsdata = response.data;
            $scope.ShipmentTotalPices = 0;
            $scope.ShipmentTotalWeight = 0;
            for (i = 0; i < $scope.gridOptionsdata.length; i++) {
                $scope.ShipmentTotalPices = $scope.ShipmentTotalPices + $scope.gridOptionsdata[i].TotalPieces;
                $scope.ShipmentTotalWeight = $scope.ShipmentTotalWeight + $scope.gridOptionsdata[i].TotalWeight;
            }

            //$scope.gridOptionsUnsuccessfuldata = response.data.UnsucessfulShipments;
            AppSpinner.hideSpinnerTemplate();
        },
     function () {
         AppSpinner.hideSpinnerTemplate();
         toaster.pop({
             type: 'error',
             title: $scope.TitleFrayteError,
             body: $scope.TextErrorOccuredDuringUpload,
             showCloseButton: true
         });
     });
    };

    $scope.rowSelectionSelectServiceCustomGrid = function (gridDirectBooking) {

        if (gridDirectBooking.IsSelected === true) {
            $scope.RemoveShipmentCount1++;
            gridDirectBooking.ServiceValue = true;
            $scope.ShipmentsWithService.push(gridDirectBooking);

        }
        else {
            $scope.IsSelected1 = false;
            for (i = 0; i < $scope.ShipmentsWithService.length; i++) {
                if (gridDirectBooking.ShipmentId === $scope.ShipmentsWithService[i].ShipmentId) {
                    $scope.ShipmentsWithService.splice(i, 1);
                    gridDirectBooking.ServiceValue = false;
                    $scope.RemoveShipmentCount1--;
                    gridDirectBooking.Service = {};
                    $scope.ServiceCount--;
                }
            }
        }
    };

    $scope.allrowSelectionSelectServiceCustomGrid = function (IsSelected1, gridDirectBooking) {

        $scope.ShipmentsWithService = [];
        if (IsSelected1 === true) {
            for (i = 0; i < gridDirectBooking.length; i++) {
                $scope.RemoveShipmentCount1++;
                gridDirectBooking[i].IsSelected = true;
                gridDirectBooking[i].ServiceValue = true;
                $scope.ShipmentsWithService.push(gridDirectBooking[i]);
            }

        }
        else {
            $scope.ShipmentsWithService = [];
            for (i = 0; i < gridDirectBooking.length; i++) {
                $scope.RemoveShipmentCount1--;
                $scope.ShipmentsWithService.splice(gridDirectBooking[i], 1);
                gridDirectBooking[i].ServiceValue = false;
                gridDirectBooking[i].IsSelected = false;
            }
        }
    };

    $scope.rowSelectionUnsuccessfulCustomGrid = function (gridDirectBooking) {

        if (gridDirectBooking.IsSelected === true) {
            $scope.RemoveShipmentCount2++;
            gridDirectBooking.ServiceValue = true;
            $scope.ShipmentsWithServiceUnsuccessful.push(gridDirectBooking);

        }
        else {
            $scope.IsSelected2 = false;
            for (i = 0; i < $scope.ShipmentsWithServiceUnsuccessful.length; i++) {
                if (gridDirectBooking.ShipmentId === $scope.ShipmentsWithServiceUnsuccessful[i].ShipmentId) {
                    $scope.ShipmentsWithServiceUnsuccessful.splice(i, 1);
                    gridDirectBooking.ServiceValue = false;
                    $scope.RemoveShipmentCount2--;
                    gridDirectBooking.Service = {};
                    $scope.ServiceCount--;
                }
            }
        }
    };

    $scope.allrowSelectionUnsuccessfullCustomGrid = function (IsSelected2, gridDirectBooking) {

        $scope.ShipmentsWithServiceUnsuccessful = [];
        if (IsSelected2 === true) {
            for (i = 0; i < gridDirectBooking.length; i++) {
                $scope.RemoveShipmentCount2++;
                gridDirectBooking[i].IsSelected = true;
                gridDirectBooking[i].ServiceValue = true;
                $scope.ShipmentsWithServiceUnsuccessful.push(gridDirectBooking[i]);
            }

        }
        else {
            $scope.ShipmentsWithService = [];
            for (i = 0; i < gridDirectBooking.length; i++) {
                $scope.RemoveShipmentCount2--;
                $scope.ShipmentsWithServiceUnsuccessful.splice(gridDirectBooking[i], 1);
                gridDirectBooking[i].ServiceValue = false;
                gridDirectBooking[i].IsSelected = false;
            }
        }
    };

    $scope.SetServiceType = function (serviceType) {
        $rootScope.ServiceType = serviceType;
    };

    $scope.AddnewShipment = function (ShipmentId) {
        if (ShipmentId > 0) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'directBookingUploadShipment/directBookingForm/directBookingForm.tpl.html',
                controller: 'DirectBookingFormController',
                windowClass: 'DirectBookingService',
                size: 'lg',
                backdrop: true,
                resolve: {
                    SessionId: function () {
                        return $scope.SessionRecord.SessionId;
                    },
                    CustomerId: function () {
                        return $scope.CustomerId;
                    },
                    ShipmentId: function () {
                        return ShipmentId;
                    }
                }
            });
            modalInstance.result.then(function () {
                $scope.GetSessionShipments($scope.SessionRecord);
            }, function () {
            });
        }
        else {
            var modalInstance1 = $uibModal.open({
                animation: true,
                templateUrl: 'directBookingUploadShipment/directBookingForm/directBookingForm.tpl.html',
                controller: 'DirectBookingFormController',
                windowClass: 'DirectBookingService',
                size: 'lg',
                backdrop: true,
                resolve: {
                    SessionId: function () {
                        return $scope.SessionRecord.SessionId;
                    },
                    CustomerId: function () {
                        return $scope.CustomerId;
                    },
                    ShipmentId: function () {
                        return 0;
                    }
                }
            });
            modalInstance1.result.then(function () {
                $scope.GetSessionShipments($scope.SessionRecord);
            }, function () {
            });
        }

    };

    $scope.GetFormatedDate = function (Date1) {
        $scope.Month = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        var Newdate = Date1;
        var newdate = [];
        var Date = [];
        var Time = [];
        var gtMonth1 = [];
        newdate = Newdate.split('T');
        Date = newdate[0].split('-');
        var gtDate = Date[2];
        gtMonth1 = Date[1].split('');
        var gtMonth2 = Date[1];
        var gtMonth3 = gtMonth1[0] === "0" ? gtMonth1[1] : gtMonth2;
        var gtMonth4 = gtMonth3--;
        var gtMonth = $scope.Month[gtMonth3];
        var gtYear = Date[0];
        $scope.result = gtDate + "-" + gtMonth + "-" + gtYear;
        return $scope.result;
    };

    $scope.GetSessionList = function () {
        DbUploadShipmentService.GetSessionList($scope.CustomerId).then(function (response) {
            if (response.data.length > 0) {
                $scope.SessionData = [];
                $scope.SessionDataList = [];
                $scope.SessionList = response.data;
                for (i = 0; i < $scope.SessionList.length; i++) {
                    $scope.SessionList[i].CreatedOn = $scope.GetFormatedDate($scope.SessionList[i].CreatedOn);

                    if ($scope.SessionList[i].SessionStatus !== "InProgress") {
                        $scope.SessionData.push($scope.SessionList[i]);
                    }
                    else if ($scope.SessionList[i].SessionStatus === "InProgress") {
                        //$scope.SessionDataList.push($scope.SessionList[i]);

                    }
                }
                if ($scope.SessionData.length === 0) {
                    $scope.flag2 = true;
                }
                $scope.SessionRecord = $scope.SessionList[$scope.SessionList.length - 1];
                $scope.GetSessionDropDownList();

            }
            else {
                $scope.flag1 = true;
            }
        },
        function () {

        });
    };

    $scope.GetSessionDropDownList = function () {
        DbUploadShipmentService.GetSessionNameList($scope.CustomerId).then(function (response) {
            if (response.data.length > 0) {
                $scope.SessionNameList = response.data;
                $scope.SessionNameList.unshift({ SessionId: 0, SessionName: 'Select Session' });
                $scope.SessionRecord = $scope.SessionNameList[$scope.SessionNameList.length - 1];
                $scope.GetSessionShipments($scope.SessionNameList[$scope.SessionNameList.length - 1]);
            }
            else {
                $scope.flag1 = true;
            }
        },
     function () {

     });
    };

    $scope.GetSessionDropLastRecord = function () {
        DbUploadShipmentService.GetSessionNameList($scope.CustomerId).then(function (response) {
            if (response.data.length > 0) {
                $scope.SessionNameList = response.data;
                $scope.SessionRecord = $scope.SessionNameList[$scope.SessionNameList.length - 1];
                $scope.GetSessionShipments($scope.SessionNameList[$scope.SessionNameList.length - 1]);
            }
        },
     function () {

     });
    };

    $scope.GetSessionShipments = function (SessionName) {
        DbUploadShipmentService.GetShipmentList(SessionName.SessionId).then(function (response) {
            if (response.data) {
                $scope.gridOptionsdata = response.data;
                $scope.ShipmentTotalPices = 0;
                $scope.ShipmentTotalWeight = 0;
                for (i = 0; i < $scope.gridOptionsdata.length; i++) {
                    $scope.ShipmentTotalPices = $scope.ShipmentTotalPices + $scope.gridOptionsdata[i].TotalPieces;
                    $scope.ShipmentTotalWeight = $scope.ShipmentTotalWeight + $scope.gridOptionsdata[i].TotalWeight;
                }
            }
        });
        $scope.Batchpercentage = 0;
    };

    $scope.GetSessionShipmentsAfterDeletion = function (SessionName) {
        DbUploadShipmentService.GetShipmentList(SessionName.SessionId).then(function (response) {
            if (response.data) {
                $scope.GridDataShipments = response.data;
                if ($scope.GridDataShipments.length > 0) {
                    $scope.GetSessionShipments($scope.SessionRecord);
                }
                else {
                    //$scope.GetSessionShipments($scope.SessionNameList[$scope.SessionNameList.length - 1]);
                    $scope.GetSessionList();
                }
            }
        });
        $scope.Batchpercentage = 0;
    };

    $scope.DeleteShipment = function (ShipmentId) {

        var modalOptions = {
            headerText: $scope.Shipment_Delete_Confirmation,
            //bodyText: "Are you sure want to delete user Details?"
            bodyText: $scope.Sure_To_Delete_Shipment_Draft
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            DbUploadShipmentService.DeleteShipment(ShipmentId).then(function (response) {
                if (response.status === 200) {
                    //$scope.SessionRecord = $scope.SessionNameList[0];
                    //$scope.GetSessionShipments($scope.SessionNameList[0]);
                    //$scope.GetSessionList();
                    $scope.GetSessionShipmentsAfterDeletion($scope.SessionRecord);
                  
                    $scope.DeletedShipment = response.data;
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.ShipmentDeletedFromSession,
                        showCloseButton: true
                    });
                }
            },
             function () {
                 toaster.pop({
                     type: 'error',
                     title: $scope.TitleFrayteError,
                     body: $scope.TextErrorOccuredDuringUpload,
                     showCloseButton: true
                 });
             });
        });

    };

    $scope.collapseClick = function () {
        $scope.isOpen = !$scope.isOpen;
    };

    function init() {
        $scope.flag1 = false;
        $scope.flag2 = false;
        $scope.PlaceBookingflag = false;
        $scope.RemoveShipmentCount1 = 0;
        $scope.RemoveShipmentCount2 = 0;
        $scope.Batchpercentage = 0;
        $scope.ShowInterval = false;
        $scope.ShipmentsWithServiceUnsuccessful = [];
        $scope.ShipmentsWithService = [];
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.LogisticService = "DHLExpress";
        $scope.ServiceType = 'DirectBooking_SS';
        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;
        $scope.CustomerId = userInfo.EmployeeId;
        $scope.OperationZoneId = userInfo.OperationZoneId;
        $scope.GetServices();
        DownloadExcelService.GetPieceDetailsExcelPath().then(function (response) {
            $scope.picesExcelDownload = response.data[0];
            $scope.customerExcelDownload = response.data[1];
            $scope.shipperExcelDownload = response.data[2];
            $scope.receiverExcelDownload = response.data[3];
            $scope.agentExcelDownload = response.data[4];
            $scope.shipmentExcelDownloadWithoutService = response.data[5];
            $scope.shipmentExcelDownloadWithService = response.data[6];
            $scope.SampleExcelWithoutService = response.data[7];
            $scope.SampleExcelWithService = response.data[8];


        }, function () {
            if (response.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorGettingSettingDetail,
                    showCloseButton: true
                });
            }
        });

        CustomerService.GetCustomerDetail($scope.CustomerId).then(function (response) {
            $scope.CustomerDetail = response.data;
            $scope.ServiceType = 'DirectBooking_SS';
            $scope.GetSessionList();
        }, function () {

        });
        DirectBookingService.GetInitials($scope.customerId).then(function (response) {
            $scope.CustDetail = response.data.CustomerDetail;
            if ($scope.RoleId === 3) {
                if ($scope.CustDetail !== null && $scope.CustDetail.IsShipperTaxAndDuty) {
                    $scope.taxAndDutyDisabled = false;
                }
                else if ($scope.CustDetail !== null && !$scope.CustDetail.IsShipperTaxAndDuty) {
                    $scope.taxAndDutyDisabled = true;
                }
                if ($scope.CustDetail !== null && $scope.CustDetail.IsRateShow) {
                    $scope.IsRateShow = true;
                }
                else {
                    $scope.IsRateShow = false;
                }
            }
            else {
                $scope.IsRateShow = true;
            }
        });
        setMultilingualOptions();
        $scope.IsCollapsedval = true;
        //$scope.isOpen = false;
        $scope.gridheight = SessionService.getScreenHeight();
        $rootScope.GetServiceValue = ' ';
    }

    init();

})
.directive('bindUnsafeHtml', [function () {
    return {
        template: '<div> + Total Shipments = {{BatchProcess}} <br> Processed Shipments = {{BatchProcessedShipments}} <br> UnProcessed Shipments  = {{BatchUnprocessedShipments}}'
    };
}]);