
angular.module('ngApp.systemAlert').
    controller('SystemAlertAddEditController', function (AppSpinner, SystemAlertService, $scope, systemAlert, systemAlertId, mode, ModalService, uiGridConstants, DirectShipmentService, config, DirectBookingService, $rootScope, $translate, $location, Upload, $state, $filter, ShipmentService, CountryService, CourierService, HomeService, ShipperService, ReceiverService, SessionService, $uibModal, $log, toaster, $stateParams, CustomerService, UserService, $uibModalInstance, TopTimeZoneService, TimeStringtoDateTime) {

        var setMultilingualOptions = function () {
            $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
                'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
            'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess',
            'Services_alertadded', 'Services_alertupdated', 'Errorwhilesaving_Alert']).then(function (translations) {

                $scope.Frayte_Warning = translations.FrayteWarning;
                $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
                $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
                $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
                $scope.Frayte_Error = translations.FrayteError;
                $scope.Frayte_Success = translations.FrayteSuccess;
                $scope.Servicesalertadded = translations.Services_alertadded;
                $scope.Services_alertupdated = translations.Services_alertupdated;
                $scope.Errorwhilesaving_Alert = translations.Errorwhilesaving_Alert;



            });
        };

        $scope.openCalender = function ($event) {
            $scope.status.opened = true;
        };

        $scope.openCalender1 = function ($event) {
            $scope.status1.opened = true;
        };
        $scope.openCalender2 = function ($event) {
            $scope.status2.opened = true;
        };
        $scope.status2 = {
            opened: false
        };
        $scope.status1 = {
            opened: false
        };

        $scope.status = {
            opened: false
        };

        $scope.ChangeFromDate = function (FromDate) {

            var newdate = [];

            newdate = new Date(FromDate);
            var gtDate = newdate.getDate();
            var gtDate1 = gtDate;
            var gtMonth = newdate.getMonth();
            var month1 = ++gtMonth;
            var gtYear = newdate.getFullYear();
            var nDate = month1 + "/" + gtDate1 + "/" + gtYear;

            $scope.systemAlertDetail.FromDate = new Date(nDate);
            return $scope.systemAlertDetail.FromDate;
        };

        $scope.ChangeToDate = function (ToDate) {

            var newdate = [];

            newdate = new Date(ToDate);
            var gtDate = newdate.getDate();
            var gtDate1 = gtDate;
            var gtMonth = newdate.getMonth();
            var month1 = ++gtMonth;
            var gtYear = newdate.getFullYear();
            var nDate = month1 + "/" + gtDate1 + "/" + gtYear;

            $scope.systemAlertDetail.ToDate = new Date(nDate);
            return $scope.systemAlertDetail.ToDate;
        };


        $scope.CheckHeading = function (Heading) {
            if ($scope.systemAlertDetail.SystemAlertId === 0) {
                SystemAlertService.SystemAlertHeadingAvailability($scope.CurrentOperationZoneId, Heading).then(function (response) {
                    if (response.data.Status === false) {
                        $scope.HeadingAvailable = true;
                        //$scope.submitted = true;
                    }
                    else {
                        $scope.HeadingAvailable = false;
                    }
                });
            }
            else if ($scope.systemAlertDetail.SystemAlertId !== 0 && $scope.OldHeading !== Heading) {
                SystemAlertService.SystemAlertHeadingAvailability($scope.CurrentOperationZoneId, Heading).then(function (response) {
                    if (response.data.Status === false) {
                        $scope.HeadingAvailable = true;
                        //$scope.submitted = true;
                    }
                    else {
                        $scope.HeadingAvailable = false;
                    }
                });
            }

        };

        $scope.DateComparision = function () {
            var TDYear;
            var TDMonth;
            var TDDate;
            var FDYear;
            var FDMonth;
            var FDDate;

            if ($scope.systemAlertDetail.ToDate !== "") {
                TDYear = $scope.systemAlertDetail.ToDate.getFullYear();
                TDMonth = $scope.systemAlertDetail.ToDate.getMonth();
                TDDate = $scope.systemAlertDetail.ToDate.getDate();
                if ($scope.systemAlertDetail.FromDate !== "") {
                    FDYear = $scope.systemAlertDetail.FromDate.getFullYear();

                    FDMonth = $scope.systemAlertDetail.FromDate.getMonth();

                    FDDate = $scope.systemAlertDetail.FromDate.getDate();
                }

                if (FDYear === TDYear && FDMonth <= TDMonth && FDDate < TDDate) {
                    $scope.CompareDate = false;
                }
                else if (FDYear === TDYear && FDMonth < TDMonth && FDDate >= TDDate) {
                    $scope.CompareDate = false;
                }
                else if (FDYear < TDYear && (FDMonth <= TDMonth || FDMonth >= TDMonth) && (FDDate < TDDate || FDMonth >= TDMonth)) {
                    $scope.CompareDate = false;
                }

                else {
                    $scope.CompareDate = true;
                }
            }

        };

        $scope.SaveSystemAlert = function (IsValid, systemAlertDetail) {
            if (IsValid) {
                $scope.systemAlertDetail.FromDate = TimeStringtoDateTime.ConvertString($scope.systemAlertDetail.FromDate, $scope.systemAlertDetail.FromTime);
                $scope.systemAlertDetail.ToDate = TimeStringtoDateTime.ConvertString($scope.systemAlertDetail.ToDate, $scope.systemAlertDetail.ToTime);

                if ($scope.systemAlertDetail.FromDate !== undefined && $scope.systemAlertDetail.FromDate !== '' &&
                     $scope.systemAlertDetail.FromDate !== null) {
                    $scope.systemAlertDetail.FromDate = moment.utc($scope.systemAlertDetail.FromDate).toDate();
                }
                if ($scope.systemAlertDetail.ToDate !== undefined && $scope.systemAlertDetail.ToDate !== '' &&
                         $scope.systemAlertDetail.ToDate !== null) {
                    $scope.systemAlertDetail.ToDate = moment.utc($scope.systemAlertDetail.ToDate).toDate();
                }
                if ($scope.systemAlertDetail.Date !== undefined && $scope.Currentdate !== '' &&
                         $scope.systemAlertDetail.Date !== null) {
                    $scope.systemAlertDetail.Date = moment.utc($scope.Currentdate).toDate();
                }
                SystemAlertService.SaveUpdateSystemAlerts($scope.systemAlertDetail).then(function (response) {
                    $rootScope.GetSystemAlert($scope.CurrentOperationZoneId);
                    //$rootScope.getAllSystemAlerts();
                    if ($scope.Mode === 'Add') {
                        toaster.pop({
                            type: 'success',
                            title: $scope.Frayte_Success,
                            body: $scope.Servicesalertadded,
                            showCloseButton: true
                        });
                    }
                    else if ($scope.Mode === 'Edit') {
                        toaster.pop({
                            type: 'success',
                            title: $scope.Frayte_Success,
                            body: $scope.Services_alertupdated,
                            showCloseButton: true
                        });
                    }
                    $uibModalInstance.close($scope.systemAlertDetail);

                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.Errorwhilesaving_Alert,
                        showCloseButton: true
                    });
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
            $scope.systemAlertDetail = {
                SystemAlertId: 0,
                Description: "",
                Heading: "",
                IsActive: false,
                OperationZoneId: null,
                TimeZoneDetail: {},
                ToDate: "",
                ToTime: "",
                FromDate: "",
                FromTime: ""
            };

            var userInfo = SessionService.getUser();

            if (mode !== undefined) {
                $translate(mode).then(function (mode) {
                    $scope.Mode = mode;
                });
            }

            $scope.Currentdate = new Date();
            $scope.customerId = userInfo.EmployeeId;
            $scope.systemAlertDetail = systemAlert;
            $scope.systemAlertDetail.FromDate1 = $scope.ChangeFromDate(systemAlert.FromDate);
            $scope.systemAlertDetail.ToDate1 = $scope.ChangeToDate(systemAlert.ToDate);
            $scope.OldHeading = $scope.systemAlertDetail.Heading;
            $scope.CompareDate = false;
            if ($scope.systemAlertDetail.FromDate !== undefined && $scope.systemAlertDetail.FromDate !== '' &&
                     $scope.systemAlertDetail.FromDate !== null) {
                $scope.systemAlertDetail.FromDate = moment.utc($scope.systemAlertDetail.FromDate).toDate();
            }
            if ($scope.systemAlertDetail.ToDate !== undefined && $scope.systemAlertDetail.ToDate !== '' &&
                     $scope.systemAlertDetail.ToDate !== null) {
                $scope.systemAlertDetail.ToDate = moment.utc($scope.systemAlertDetail.ToDate).toDate();
            }
            if ($scope.systemAlertDetail.Date !== undefined && $scope.Currentdate !== '' &&
                     $scope.systemAlertDetail.Date !== null) {
                $scope.systemAlertDetail.Date = moment.utc($scope.Currentdate).toDate();
            }

            $scope.dateOptions = {
                formatYear: 'yy',
                startingDay: 1
            };
            HomeService.GetCurrentOperationZone().then(function (response) {
                $scope.CurrentOperationZoneId = response.data.OperationZoneId;
            });
            //SystemAlertService.GetTimeZones().then(function (response) {
            //    $scope.TimeZoneList = response.data.TimeZones;

            //    //for (j = 0; j < $scope.TimeZoneList.length; j++) {
            //    //    $scope.TimeZoneList[j].OffsetShort = $scope.TimeZoneList[j].Name + " " + $scope.TimeZoneList[j].OffsetShort;
            //    //}


            //    if (mode === 'Edit') {
            //        for (i = 0; i < $scope.TimeZoneList.length; i++) {
            //            if ($scope.systemAlertDetail.TimeZoneDetail.TimezoneId === $scope.TimeZoneList[i].TimezoneId) {
            //                $scope.TimeZoneList[i].OffsetShort = $scope.systemAlertDetail.TimeZoneDetail.OffsetShort;


            //            }
            //        }
            //    }

            //});
            DirectBookingService.GetInitials($scope.customerId).then(function (response) {
                // Set Country type according to given order
                //$scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
                $scope.TimeZoneList = TopTimeZoneService.TopTimeZoneOrder(response.data.Countries);
                if (mode === 'Edit') {
                    for (i = 0; i < $scope.TimeZoneList.length; i++) {
                        if ($scope.systemAlertDetail.TimeZoneDetail.TimezoneId === $scope.TimeZoneList[i].TimezoneId) {
                            $scope.TimeZoneList[i].OffsetShort = $scope.systemAlertDetail.TimeZoneDetail.OffsetShort;
                        }
                    }
                }
            });
            setMultilingualOptions();
        }

        init();
    });
