angular.module('ngApp.customer').controller('TradelaneTrackingConfigurationController', function ($scope, config, $state, TrackAndTraceDashboardService, AppSpinner, $location, $translate, $stateParams, $filter, CustomerService, SessionService, $uibModal, uiGridConstants, toaster, ModalService, TradelaneService, CarrierService, CountryService, ShipmentService, UserService, $anchorScroll, TradelaneMilestoneService) {

    // set translation key
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteSuccess', 'FrayteWarning', 'ErrorSavingRecored', 'SavedRecordsSuccessfully', 'LoadingTrackingConfiguration', 'PleaseCorrectValidationErrors', 'ErrorGettingRecordPleaseTryAgain',
        'Delete_Tracking_Configuration_Detail', 'Tracking_Configuration_Delete_Confirmation', 'Removed_Successfully', 'Tracking_Configuration_Detail_Saved_Successfully', 'NoRecordSelectedShipmentHandler',
        'ErrorGettingRecords']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteValidation = translations.FrayteWarning;
            $scope.ErrorSavingRecored = translations.ErrorSavingRecored;
            $scope.SavedRecordsSuccessfully = translations.SavedRecordsSuccessfully;
            $scope.LoadingTrackingConfiguration = translations.LoadingTrackingConfiguration;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.ErrorGettingRecord_PleaseTryAgain = translations.ErrorGettingRecordPleaseTryAgain;
            $scope.Delete_Tracking_Configuration_Detail = translations.Delete_Tracking_Configuration_Detail;
            $scope.Tracking_Configuration_Delete_Confirmation = translations.Tracking_Configuration_Delete_Confirmation;
            $scope.Removed_Successfully = translations.Removed_Successfully;
            $scope.Tracking_Configuration_Detail_Saved_Successfully = translations.Tracking_Configuration_Detail_Saved_Successfully;
            $scope.NoRecordSelectedShipmentHandler = translations.NoRecordSelectedShipmentHandler;
            $scope.ErrorGettingRecords = translations.ErrorGettingRecords;
            // Initial Deatil call here --> so that Spinner Message should be multilingual 
            $scope.GetInitial();
        });
    };

    var setTrackingJson = function () {
        $scope.TrackingconfigurationJson = {
            TrackingMileStone: [],
            PreAlert: {}
        };
        $scope.MileStoneJson = {
            TrackingMileStoneId: 0,
            UserId: $scope.param.customerId,
            IsEmailSend: false,
            Description: "",
            ConfigurationDetail: []
        };
        $scope.PreAlert = {
            OtherMethod: "PreAlert",
            UserId: $scope.param.customerId,
            IsEmailSend: false,
            ConfigurationDetail: []
        };
        var ConfigDetail = {
            Name: "",
            Email: "",
            CreatedBy: 0,
            UpdatedOn: 0
        };
        $scope.TrackingconfigurationJson.PreAlert = $scope.PreAlert;
        $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail.push(ConfigDetail);
        for (j = 0; j < $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail.length; j++) {

            //if (j == $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail.length - 1) {
            if (j === 0 || j === $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length - 1) {
                $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail[j].pacVal = true;
            }
            else {
                $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail[j].pacVal = false;

            }
        }
        ConfigDetail = {
            Name: "",
            Email: "",
            CreatedBy: 0,
            UpdatedOn: 0
        };
        for (i = 0; i < $scope.TrackingMileStones.length; i++) {
            $scope.MileStoneJson.TrackingMileStoneId = $scope.TrackingMileStones[i].TrackingMileStoneId;
            $scope.MileStoneJson.UserId = $scope.param.customerId;
            $scope.MileStoneJson.IsEmailSend = false;
            $scope.MileStoneJson.Description = $scope.TrackingMileStones[i].Description;
            $scope.MileStoneJson.MileStoneKey = $scope.TrackingMileStones[i].MileStoneKey;
            $scope.TrackingconfigurationJson.TrackingMileStone.push($scope.MileStoneJson);
            $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.push(ConfigDetail);
            $scope.MileStoneJson = {
                TrackingMileStoneId: 0,
                UserId: $scope.param.customerId,
                IsEmailSend: false,
                ConfigurationDetail: []
            };
            ConfigDetail = {
                TradelaneUserTrackingConfigurationDetailId: 0,
                Name: "",
                Email: "",
                CreatedBy: 0,
                UpdatedOn: 0
            };
        }
        for (i = 0; i < $scope.TrackingconfigurationJson.TrackingMileStone.length; i++) {
            for (j = 0; j < $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length; j++) {
                if (j === 0 || j === $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length - 1) {
                    $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail[j].pacVal = true;
                }
                else if ($scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length > 1) {
                    $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail[j].pacVal = false;
                }
            }
        }
    };
    var setTrackingJsonWithPreAlert = function () {
        var Prealert = angular.copy($scope.TrackingconfigurationJson.PreAlert);
        $scope.TrackingconfigurationJson = {
            TrackingMileStone: [],
            PreAlert: Prealert
        };
        $scope.MileStoneJson = {
            TrackingMileStoneId: 0,
            UserId: $scope.param.customerId,
            IsEmailSend: false,
            Description: "",
            ConfigurationDetail: []
        };
        var ConfigDetail = {
            TradelaneUserTrackingConfigurationDetailId: 0,
            Name: "",
            Email: "",
            CreatedBy: 0,
            UpdatedOn: 0
        };
        for (i = 0; i < $scope.TrackingMileStones.length; i++) {
            ConfigDetail = {
                TradelaneUserTrackingConfigurationDetailId: 0,
                Name: "",
                Email: "",
                CreatedBy: 0,
                UpdatedOn: 0
            };
            $scope.MileStoneJson.TrackingMileStoneId = $scope.TrackingMileStones[i].TrackingMileStoneId;
            $scope.MileStoneJson.UserId = $scope.param.customerId;
            $scope.MileStoneJson.IsEmailSend = false;
            $scope.MileStoneJson.Description = $scope.TrackingMileStones[i].Description;
            $scope.MileStoneJson.MileStoneKey = $scope.TrackingMileStones[i].MileStoneKey;
            $scope.TrackingconfigurationJson.TrackingMileStone.push($scope.MileStoneJson);
            $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.push(ConfigDetail);
            $scope.MileStoneJson = {
                TrackingMileStoneId: 0,
                UserId: $scope.param.customerId,
                IsEmailSend: false,
                ConfigurationDetail: []
            };
            ConfigDetail = {
                TradelaneUserTrackingConfigurationDetailId: 0,
                Name: "",
                Email: "",
                CreatedBy: 0,
                UpdatedOn: 0
            };
        }
        for (i = 0; i < $scope.TrackingconfigurationJson.TrackingMileStone.length; i++) {
            for (j = 0; j < $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length; j++) {
                if (j === 0 || j === $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length - 1) {
                    $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail[j].pacVal = true;
                }
                else if ($scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length > 1) {
                    $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail[j].pacVal = false;
                }
            }
        }
    };
    var setTrackingMileStoneJson = function () {
        $scope.MileStoneJson = {
            TrackingMileStoneId: 0,
            UserId: $scope.param.customerId,
            IsEmailSend: false,
            Description: "",
            ConfigurationDetail: []
        };

        var ConfigDetail = {
            Name: "",
            Email: "",
            CreatedBy: 0,
            UpdatedOn: 0
        };
        for (i = 0; i < $scope.TrackingMileStones.length; i++) {
            $scope.MileStoneJson.TrackingMileStoneId = $scope.TrackingMileStones[i].TrackingMileStoneId;
            $scope.MileStoneJson.UserId = $scope.param.customerId;
            $scope.MileStoneJson.IsEmailSend = false;
            $scope.MileStoneJson.Description = $scope.TrackingMileStones[i].Description;
            $scope.MileStoneJson.MileStoneKey = $scope.TrackingMileStones[i].MileStoneKey;
            $scope.TrackingconfigurationJson.TrackingMileStone.push($scope.MileStoneJson);
            $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.push(ConfigDetail);
            $scope.MileStoneJson = {
                TrackingMileStoneId: 0,
                UserId: $scope.param.customerId,
                IsEmailSend: false,
                ConfigurationDetail: []
            };
            ConfigDetail = {
                TradelaneUserTrackingConfigurationDetailId: 0,
                Name: "",
                Email: "",
                CreatedBy: 0,
                UpdatedOn: 0
            };
        }
        for (i = 0; i < $scope.TrackingconfigurationJson.TrackingMileStone.length; i++) {
            for (j = 0; j < $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length; j++) {
                if (j === 0 || j === $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length - 1) {
                    $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail[j].pacVal = true;
                }
                else {
                    $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail[j].pacVal = false;
                }
            }
        }
    };

    $scope.AddPackage = function (MileStone) {
        MileStone.ConfigurationDetail.push({
            TradlaneUserTrackingConfigurationDetailId: 0,
            Name: "",
            Email: "",
            CreatedBy: 0,
            UpdatedOn: 0
        });
        //var dbpac = MileStone.ConfigurationDetail.length - 1;
        for (i = 0; i < MileStone.ConfigurationDetail.length; i++) {
            if (i === 0 || i === MileStone.ConfigurationDetail.length - 1) {
                MileStone.ConfigurationDetail[i].pacVal = true;
            }
            else {
                MileStone.ConfigurationDetail[i].pacVal = false;
            }
        }
    };

    $scope.RemovePackage = function (MileStone, ConfigRecord) {
        if (ConfigRecord !== undefined && ConfigRecord !== null) {
            var index = MileStone.ConfigurationDetail.indexOf(ConfigRecord);
            if (MileStone.ConfigurationDetail.length === 2) {
                $scope.HideContent = false;
            }
            if (ConfigRecord.TradelaneUserTrackingConfigurationDetailId > 0) {
                var modalOptions = {
                    headerText: $scope.Tracking_Configuration_Delete_Confirmation,
                    bodyText: $scope.Delete_Tracking_Configuration_Detail
                };

                ModalService.Confirm({}, modalOptions).then(function (result) {
                    CustomerService.DeleteConfigRecord(ConfigRecord.TradelaneUserTrackingConfigurationDetailId).then(function (response) {
                        if (response.data) {
                            toaster.pop({
                                type: 'success',
                                title: $scope.FrayteSuccess,
                                body: $scope.Removed_Successfully,
                                showCloseButton: true
                            });
                            MileStone.ConfigurationDetail.splice(index, 1);
                            $scope.Packges = angular.copy(MileStone.ConfigurationDetail);
                            MileStone.ConfigurationDetail = [];
                            MileStone.ConfigurationDetail = $scope.Packges;
                            //$scope.GetInitial();
                            if (MileStone.ConfigurationDetail.length === 0) {
                                MileStone.ConfigurationDetail.push({
                                    TrackingMileStoneId: 0,
                                    UserId: $scope.param.customerId,
                                    IsEmailSend: false,
                                    Description: "",
                                    ConfigurationDetail: []
                                });
                                for (i = 0; i < MileStone.ConfigurationDetail.length; i++) {
                                    MileStone.ConfigurationDetail[i].pacVal = true;
                                }
                            }
                            else {
                                for (i = 0; i < MileStone.ConfigurationDetail.length; i++) {

                                    if (i === 0 || i === MileStone.ConfigurationDetail.length - 1) {
                                        MileStone.ConfigurationDetail[i].pacVal = true;
                                    }
                                    else {
                                        MileStone.ConfigurationDetail[i].pacVal = false;
                                    }
                                }
                            }
                        }

                    }, function () {
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteError,
                            body: $scope.RemovePackageValidation,
                            showCloseButton: true
                        });
                    });
                });
            }
            else {
                MileStone.ConfigurationDetail.splice(index, 1);
                $scope.Packges = angular.copy(MileStone.ConfigurationDetail);
                MileStone.ConfigurationDetail = [];
                MileStone.ConfigurationDetail = $scope.Packges;

                if (index === MileStone.ConfigurationDetail.length - 1) {
                    var dbpac = MileStone.ConfigurationDetail.length - 2;
                    for (i = 0; i < MileStone.ConfigurationDetail.length; i++) {

                        if (i === 0 || i === MileStone.ConfigurationDetail.length - 1) {
                            MileStone.ConfigurationDetail[i].pacVal = true;
                        }
                        else {
                            MileStone.ConfigurationDetail[i].pacVal = false;
                        }
                    }
                }
                else {
                    var dbpac1 = MileStone.ConfigurationDetail.length - 1;
                    for (i = 0; i < MileStone.ConfigurationDetail.length; i++) {

                        if (i === 0 || i === MileStone.ConfigurationDetail.length - 1) {
                            MileStone.ConfigurationDetail[i].pacVal = true;
                        }
                        else {
                            MileStone.ConfigurationDetail[i].pacVal = false;
                        }
                    }
                }
                if (MileStone.ConfigurationDetail.length === 0) {
                    MileStone.ConfigurationDetail.push({
                        TrackingMileStoneId: 0,
                        UserId: $scope.param.customerId,
                        IsEmailSend: false,
                        Description: "",
                        ConfigurationDetail: []
                    });
                    for (i = 0; i < MileStone.ConfigurationDetail.length; i++) {
                        MileStone.ConfigurationDetail[i].pacVal = true;
                    }
                }
            }
        }
    };

    $scope.SaveTrackingConfiguration = function (IsValid, TrackingConfigurationJson) {
        if (IsValid) {
            CustomerService.SaveTradelaneTrackingConfiguration($scope.TrackingconfigurationJson).then(function (response) {
                if (response.data) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.Tracking_Configuration_Detail_Saved_Successfully,
                        showCloseButton: true
                    });
                    //$scope.getTrackingMileStones();
                    $scope.GetInitial();
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingRecord_PleaseTryAgain,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.PleaseCorrectValidationErrors,
                showCloseButton: true
            });
        }
    };

    $scope.SetTrackingConfigEditJson = function () {
        var ConfigDetail = {
            Name: "",
            Email: "",
            CreatedBy: 0,
            UpdatedOn: 0
        };

        //for (i = 0; i < $scope.TrackingMileStones.length; i++) {
        //    var flag = false;
        //    //for (j = 0; j < $scope.TrackingconfigurationJson.TrackingMileStone.length; j++) {
        //        //if ($scope.TrackingMileStones[i].TrackingMileStoneId == $scope.TrackingconfigurationJson.TrackingMileStone[j].TrackingMileStoneId) {
        //        //    flag = true;
        //        //    $scope.TrackingconfigurationJson.TrackingMileStone[j].Description = $scope.TrackingMileStones[i].Description;
        //        //    $scope.TrackingconfigurationJson.TrackingMileStone[j].MileStoneKey = $scope.TrackingMileStones[i].MileStoneKey;
        //        //}
        //        //else {

        //        //}
        //    //}
        //    //if (!flag) {

        //        $scope.MileStoneJson = {
        //            TrackingMileStoneId: $scope.TrackingMileStones[i].TrackingMileStoneId,
        //            UserId: $scope.param.customerId,
        //            IsEmailSend: false,
        //            ConfigurationDetail: []
        //        };
        //        $scope.TrackingconfigurationJson.TrackingMileStone.push($scope.MileStoneJson);
        //        $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.push(ConfigDetail);
        //        $scope.TrackingconfigurationJson.TrackingMileStone[i].Description = $scope.TrackingMileStones[i].Description;
        //        $scope.TrackingconfigurationJson.TrackingMileStone[i].MileStoneKey = $scope.TrackingMileStones[i].MileStoneKey;
        //    //}
        //}
        $scope.TrackingconfigurationJson.TrackingMileStone = [];
        for (i = 0; i < $scope.TrackingMileStones.length; i++) {
            var flag = false;
            $scope.MileStoneJson = {
                TrackingMileStoneId: $scope.TrackingMileStones[i].TrackingMileStoneId,
                UserId: $scope.param.customerId,
                IsEmailSend: false,
                ConfigurationDetail: []
            };
            $scope.TrackingconfigurationJson.TrackingMileStone.push($scope.MileStoneJson);
            $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.push(ConfigDetail);
            $scope.TrackingconfigurationJson.TrackingMileStone[i].Description = $scope.TrackingMileStones[i].Description;
            $scope.TrackingconfigurationJson.TrackingMileStone[i].MileStoneKey = $scope.TrackingMileStones[i].MileStoneKey;
            ConfigDetail = {
                Name: "",
                Email: "",
                CreatedBy: 0,
                UpdatedOn: 0
            };
        }

        for (ii = 0; ii < $scope.TrackingMileStoneData.length; ii++) {
            for (j = 0; j < $scope.TrackingMileStones.length; j++) {
                if ($scope.TrackingMileStoneData[ii].TrackingMileStoneId == $scope.TrackingMileStones[j].TrackingMileStoneId) {
                    $scope.TrackingconfigurationJson.TrackingMileStone[j].IsEmailSend = $scope.TrackingMileStoneData[ii].IsEmailSend;
                    $scope.TrackingconfigurationJson.TrackingMileStone[j].TradelaneUserTrackingConfigurationId = $scope.TrackingMileStoneData[ii].TradelaneUserTrackingConfigurationId;
                    $scope.TrackingconfigurationJson.TrackingMileStone[j].ConfigurationDetail = [];
                    for (jk = 0; jk < $scope.TrackingMileStoneData[ii].ConfigurationDetail.length; jk++) {
                        $scope.TrackingconfigurationJson.TrackingMileStone[j].ConfigurationDetail.push($scope.TrackingMileStoneData[ii].ConfigurationDetail[jk]);
                    }
                }

            }
        }
  
        for (jj = 0; jj < $scope.TrackingconfigurationJson.TrackingMileStone.length; jj++) {
            if ($scope.TrackingconfigurationJson.TrackingMileStone[jj].ConfigurationDetail.length === 0) {
                $scope.TrackingconfigurationJson.TrackingMileStone[jj].ConfigurationDetail.push({
                    TrackingMileStoneId: $scope.TrackingconfigurationJson.TrackingMileStone[jj].TrackingMileStoneId,
                    UserId: $scope.param.customerId,
                    IsEmailSend: false,
                    ConfigurationDetail: []
                });
            }
        }

        for (a = 0; a < $scope.TrackingconfigurationJson.TrackingMileStone.length; a++) {
            for (j = 0; j < $scope.TrackingconfigurationJson.TrackingMileStone[a].ConfigurationDetail.length; j++) {
                if (j === 0 || j === $scope.TrackingconfigurationJson.TrackingMileStone[a].ConfigurationDetail.length - 1) {
                    $scope.TrackingconfigurationJson.TrackingMileStone[a].ConfigurationDetail[j].pacVal = true;
                }
                else {
                    $scope.TrackingconfigurationJson.TrackingMileStone[a].ConfigurationDetail[j].pacVal = false;
                }
            }
        }
    };

    $scope.ChangeTrackingConfig = function () {
        TradelaneMilestoneService.GetTrackingMileStone($scope.ShipmentHandlerMethod.ShipmentHandlerMethodId).then(function (response) {
            if (response.data.length > 0) {
                var TrackIds = "";
                $scope.TrackingMileStones = response.data;
                for (i = 0; i < $scope.TrackingMileStones.length; i++) {
                    if (i === 0) {
                        TrackIds = $scope.TrackingMileStones[i].TrackingMileStoneId;
                    }
                    else {
                        TrackIds = TrackIds + ',' + $scope.TrackingMileStones[i].TrackingMileStoneId;
                    }

                }
                $scope.Flag2 = false;

                CustomerService.GetTradelaneTrackingConfiguration($scope.param.customerId, TrackIds).then(function (response) {
                    if (response.data != null) {
                        //$scope.TrackingconfigurationJson.TrackingMileStone = response.data;

                        $scope.TrackingMileStoneData = response.data;
                        //if ($scope.TrackingconfigurationJson.PreAlert === null) {
                        //    CustomerService.GetPreAlert($scope.param.customerId).then(function (res) {
                        //        if (res.data) {
                        //            $scope.TrackingconfigurationJson.PreAlert = res.data;
                        //        }

                        //    }, function () {

                        //    });
                        //}
                        $scope.SetTrackingConfigEditJson();
                        for (i = 0; i < $scope.TrackingconfigurationJson.TrackingMileStone.length; i++) {
                            for (j = 0; j < $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length; j++) {
                                if (j === 0 || j === $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail.length - 1) {
                                    $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail[j].pacVal = true;
                                }
                                else {
                                    $scope.TrackingconfigurationJson.TrackingMileStone[i].ConfigurationDetail[j].pacVal = false;
                                }
                            }
                        }
                    }
                    else {
                        $scope.TrackingconfigurationJson.TrackingMileStone = [];
                        setTrackingMileStoneJson();
                        //$scope.Flag2 = true;
                    }
                    //if (response.data.PreAlert === null) {
                    //    CustomerService.GetPreAlert($scope.param.customerId).then(function (res) {
                    //        if (res.data) {
                    //            //setTrackingJson();
                    //            $scope.TrackingconfigurationJson.PreAlert = res.data;
                    //        }

                    //    }, function () {

                    //    });
                    //}
                });
            }
            else {
                $scope.TrackingMileStones = response.data;
                $scope.TrackingconfigurationJson.TrackingMileStone = [];
                $scope.Flag2 = true;
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: $scope.NoRecordSelectedShipmentHandler,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorGettingRecord_PleaseTryAgain,
                showCloseButton: true
            });
        });
    };

    $scope.getTrackingMileStones = function () {
        TradelaneMilestoneService.GetTrackingMileStone($scope.ShipmentHandlerMethod.ShipmentHandlerMethodId).then(function (response) {
            if (response.data.length > 0) {
                var TrackIds = "";
                $scope.TrackingMileStones = response.data;
                for (i = 0; i < $scope.TrackingMileStones.length; i++) {
                    if (i === 0) {
                        TrackIds = $scope.TrackingMileStones[i].TrackingMileStoneId;
                    }
                    else {
                        TrackIds = TrackIds + ',' + $scope.TrackingMileStones[i].TrackingMileStoneId;
                    }

                }
                $scope.Flag2 = false;

                CustomerService.GetTradelaneTrackingConfiguration($scope.param.customerId, TrackIds).then(function (response) {
                    if (response.data != null) {
                        //$scope.TrackingconfigurationJson.TrackingMileStone = response.data;
                        $scope.TrackingMileStoneData = response.data;
                        $scope.SetTrackingConfigEditJson();
                    }
                    else if ($scope.TrackingconfigurationJson.PreAlert != null && $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail !== undefined && $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail.length > 0) {
                        setTrackingJsonWithPreAlert();
                    }
                    else {
                        setTrackingJson();
                    }
                });
            }
            else {
                $scope.TrackingMileStones = response.data;
                $scope.TrackingconfigurationJson = {};
                $scope.Flag2 = true;
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: $scope.NoRecordSelectedShipmentHandler,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorGettingRecords,
                showCloseButton: true
            });
        });
    };

    $scope.ValidStartTime = function (starttime) {
        if (starttime !== undefined && starttime !== null && starttime !== '') {
            if (parseInt(starttime, 0) > 2359) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: $scope.ValidTime,
                    showCloseButton: true
                });
                $scope.userDetail.WorkingStartTime = null;
            }
        }
    };


    $scope.GetInitial = function () {
        TradelaneMilestoneService.GetShipmentHandlerMethods().then(function (response) {
            if (response.data.length > 0) {
                $scope.ShipmentHandlerList = response.data;
                $scope.ShipmentHandlerMethod = $scope.ShipmentHandlerList[0];

                CustomerService.GetPreAlert($scope.param.customerId).then(function (res) {
                    if (res.data !== null && res.data.TradelaneUserTrackingConfigurationId !== 0) {
                        $scope.TrackingconfigurationJson.PreAlert = res.data;
                        if ($scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail.length === 0) {
                            $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail.push({
                                TrackingMileStoneId: 0,
                                UserId: $scope.param.customerId,
                                IsEmailSend: false,
                                Description: "",
                                ConfigurationDetail: []
                            });
                        }
                        for (j = 0; j < $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail.length; j++) {
                            if (j === 0 || j === $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail.length - 1) {
                                $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail[j].pacVal = true;
                            }
                            else {
                                $scope.TrackingconfigurationJson.PreAlert.ConfigurationDetail[j].pacVal = false;

                            }
                        }
                        $scope.getTrackingMileStones();
                    }
                    else {
                        $scope.getTrackingMileStones();
                    }

                }, function () {

                });
            }
            else {
                $scope.ShipmentHandlerList = response.data;
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: $scope.NoRecordShipmentHandlerMethods,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorGettingRecords,
                showCloseButton: true
            });
        });
    };

    function init() {
        $scope.submitted = true;
        $scope.emailFormat = /^[a-z0-9._]+@[a-z]+\.[a-z.]/;
        $scope.Flag2 = false;
        $scope.TrackingconfigurationJson = {
            PreAlert: {},
            TrackingMileStone: []
        };
        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;
        $scope.CustomerId = userInfo.EmployeeId;
        $scope.param = $state.params;
        CustomerService.GetCustomerDetail($scope.CustomerId).then(function (response) {
            $scope.CustomerDetail = response.data;
        }, function () {

        });
        setMultilingualOptions();
    }

    init();
});