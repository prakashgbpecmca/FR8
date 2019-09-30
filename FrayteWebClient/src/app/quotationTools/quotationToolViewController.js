angular.module('ngApp.quotationTools').controller('QuotationToolViewController', function ($scope, $state, $rootScope, QuotationService, OperationZoneId, UserId, $http, config, $window, toaster, AppSpinner, uiGridConstants, $translate, DirectBookingService, ModalService, $uibModalInstance, SessionService, $uibModal) {


    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
            'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
        'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation',
        'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess', 'Quotationcannotplaced_offervalidityexpired', 'Report_GeneratedDownloaded_Successfully',
        'Could_Not_Download_TheReport', 'Error_while_downloading', 'SendingMailError_Validation', 'SuccessfullySentMail']).then(function (translations) {
            
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
            $scope.Frayte_Error = translations.FrayteError;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.Quotationoffervalidityexpired = translations.Quotationcannotplaced_offervalidityexpired;
            $scope.Report_GeneratedDownloadedSuccessfully = translations.Report_GeneratedDownloaded_Successfully;
            $scope.CouldNot_Download_TheReport = translations.Could_Not_Download_TheReport;
            $scope.Errorwhile_downloading = translations.Error_while_downloading;
            $scope.SendingMailErrorValidation = translations.SendingMailError_Validation;
            $scope.Successfully_SentMail = translations.SuccessfullySentMail;
            $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;


        });
    };
    var getPlaceBookinLink = function (QuotationShipmentId) {
        //QuotationService.GetPlaceBookingLink(QuotationShipmentId, 'ShipmentQuotation').then(function (response) {
            $uibModalInstance.close();
        //$scope.PlaceBookingLink = response.data;

            QuotationService.GetQuotationValidity(QuotationShipmentId).then(function (response) {
                if (response.data.Status === false) {
                    if ($scope.RoleId === 1) {
                        $state.go('admin.booking-home.direct-booking', { callingtype: 'quotation-shipment', directShipmentId: QuotationShipmentId }, { reload: true });
                    }
                    else if ($scope.RoleId === 6) {
                        $state.go('dbuser.booking-home.direct-booking', { callingtype: 'quotation-shipment', directShipmentId: QuotationShipmentId }, { reload: true });
                    }
                    else if ($scope.RoleId === 3) {
                        $state.go('customer.booking-home.direct-booking', { callingtype: 'quotation-shipment', directShipmentId: QuotationShipmentId }, { reload: true });
                    }
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.Quotationoffervalidityexpired,
                        showCloseButton: true
                    });
                }
            });
           
            //$rootScope.directBooking = $scope.PlaceBookingLink;
        //});
    };

    $scope.ConfirmQuotation = function (row) {
        if (row !== undefined && row !== null) {
            getPlaceBookinLink(row.QuotationShipmentId);
        }
    };
    $scope.toggleRowSelect = function (data) {
        if (data) {
            angular.forEach($scope.gridData, function (eachObj) {
                eachObj.RowSelect = false;
            });
            data.RowSelect = true;
        }
    };
    var getScreenInitials = function () {
        AppSpinner.showSpinnerTemplate("Loading Quotes", $scope.Template);
        QuotationService.GetQuotationShipments($scope.OperationZoneId, $scope.UserId).then(function (response) {
            if (response.data !== null) {
                for (i = 0; i < response.data.length; i++){
                    //response.data[i].EstimatedCost = response.data[i].EstimatedCost.toFixed(2);
                    //response.data[i].FuelSurCharge = response.data[i].FuelSurCharge.toFixed(2);
                    //response.data[i].EstimatedTotalCost = response.data[i].EstimatedTotalCost.toFixed(2);
                    //response.data[i].AdditionalSurcharge = response.data[i].AdditionalSurcharge.toFixed(2);

                    response.data[i].EstimatedCost = Number(parseFloat(response.data[i].EstimatedCost).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                    response.data[i].FuelSurCharge = response.data[i].FuelSurCharge.toFixed(2);
                    response.data[i].EstimatedTotalCost = Number(parseFloat(response.data[i].EstimatedTotalCost).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                    response.data[i].AdditionalSurcharge = Number(parseFloat(response.data[i].AdditionalSurcharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                }
                $scope.gridData = response.data;
                for (var i = 0; i < $scope.gridData.length; i++) {
                    $scope.gridData[i].collapseToggle = false;
                    $scope.gridData[i].RowSelect = false;
                    $scope.gridData[i].collapse = true;
                }
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });

    };

    $scope.DownlaodQuoteReport = function (quotationDetail) {
        $rootScope.GetServiceValue = null;
        AppSpinner.showSpinnerTemplate("Downloading Quote PDF", $scope.Template);
        QuotationService.GenerateQuotationShipmentPdf(quotationDetail).then(function (response) {
            var fileInfo = response.data;
            var fileName = {
                FileName: response.data.FileName
            };
            if (response.data != null) {
                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/Quotation/DownloadQuotationReport',
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
                            AppSpinner.hideSpinnerTemplate();
                            toaster.pop({
                                type: 'success',
                                title: $scope.Frayte_Success,
                                body: $scope.Report_GeneratedDownloadedSuccessfully,
                                showCloseButton: true
                            });
                        } catch (ex) {
                            AppSpinner.hideSpinnerTemplate();
                            $window.open(fileInfo.FilePath, "_blank");
                            toaster.pop({
                                type: 'success',
                                title: $scope.Frayte_Success,
                                body: $scope.Report_GeneratedDownloadedSuccessfully,
                                showCloseButton: true
                            });
                        }

                    }
                })
               .error(function (data) {
                   AppSpinner.hideSpinnerTemplate();
                   toaster.pop({
                       type: 'error',
                       title: $scope.Frayte_Error,
                       body: $scope.CouldNot_Download_TheReport,
                       showCloseButton: true
                   });
               });

            }
            else {
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.Errorwhile_downloading,
                showCloseButton: true
            });
        });
    };

    $scope.SendQuotationMail = function (quotationDetail) {
        var data = {
            Name: $scope.CustomerName,
            Email: $scope.Email,
            QuotationDetail: quotationDetail
        };
        QuotationService.SendQuotationMail(data).then(function (response) {
            if (response.status === 200) {
                toaster.pop({
                    type: 'success',
                    title: $scope.Frayte_Success,
                    body: $scope.Successfully_SentMail,
                    showCloseButton: true
                });

                //  $scope.NewQuotation();
            }
            else {
                //  $scope.NewQuotation();
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: $scope.SendingMailErrorValidation,
                    showCloseButton: true
                });
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.SendingMailErrorValidation,
                showCloseButton: true
            });
        });



    };

    $scope.SendEmailCustomer = function (item) {

        if ($scope.quotationDetail !== null && item !== null && item !== undefined) {
            //call to get sales representative
            QuotationService.GetSalesRepresentiveEmail(item.CustomerId).then(function (response) {
                $scope.SalesRepresentativedata = {
                    Name: '',
                    Email: ''
                };
                $scope.SalesRepresentative = response.data;
                if (response.data === null) {
                    $scope.SalesRepresentativedata.Name = $scope.UserName;
                    $scope.SalesRepresentativedata.Email = $scope.UserEmail;
                }
                else {
                    $scope.SalesRepresentativedata.Name = $scope.SalesRepresentative.SalesRepresentiveName;
                    $scope.SalesRepresentativedata.Email = $scope.SalesRepresentative.SalesEmail;
                }
                if ($scope.SalesRepresentativedata !== null && $scope.SalesRepresentativedata !== undefined) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'quotationTools/quotationToolEmailquote.tpl.html',
                        controller: 'QuotationToolMailController',
                        windowClass: '',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            quotationDetail: function () {
                                return $scope.quotationDetail;
                            },
                            item: function () {
                                return item;
                            },
                            CustomerName: function () {
                                return item.CustomerName;
                            },
                            CustomerEmail: function () {
                                return item.CustomerEmail;
                            },
                            CustomerDetail: function () {
                                return $scope.CustomerDetail;
                            },
                            CustId: function () {
                                return $scope.CustId;
                            },
                            CustName: function () {
                                return $scope.CustName;
                            },
                            CustEmail: function () {
                                return $scope.CustEmail;
                            },
                            Header: function () {
                                return 'Send My Quote Email';
                            },
                            MailContentText: function () {
                                return 'Many thanks for requesting an Air Freight Quote for an ad hoc shipment at FRAYTE Logistics Ltd. It is our pleasure to present Your Quote as per the following attachment. Thank you very much and please feel free to let us know if you have any questions.';
                            },
                            SalesRepresentativeDetail: function () {
                                return $scope.SalesRepresentativedata;
                            }
                        }
                    });

                    modalInstance.result.then(function () {

                    }, function () {

                    });
                }
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrect_ValidationErrors,
                showCloseButton: true
            });
        }

    };
    function init() {
        $scope.userInfo = SessionService.getUser();
        $scope.UserName = $scope.userInfo.UserName;
        $scope.UserEmail = $scope.userInfo.EmployeeMail;
        $scope.RoleId = $scope.userInfo.RoleId;
        $scope.UserId = UserId;
        $scope.OperationZoneId = OperationZoneId;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        getScreenInitials();
        setMultilingualOptions();
    }

    init();

});
