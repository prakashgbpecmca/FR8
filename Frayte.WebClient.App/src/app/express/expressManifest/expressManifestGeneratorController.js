angular.module('ngApp.express').controller("expressManifestGeneratorController", function ($scope, $uibModal, ModalService, config, SessionService, ExpressShipmentService, toaster, ExpressManifestService, DateFormatChange, $translate, AppSpinner, $timeout) {

    var setModalOptions = function () {
        $translate(['Frayte_Error', 'FrayteWarning', 'FrayteSuccess', 'SelectAtleast_OneManifest', 'SelectAtleast_OneHub', 'SelectAtleast_OneCustomer', 'ErrorWhileCreating_Manifest', 'Error', 'SUCCESS', 'Manifest_Created_Successfully',
        'CreatingManifest', 'LoadingNonManifestShipments']).then(function (translations) {
            $scope.FrayteError = translations.Frayte_Error;
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.SelectAtleastOneManifest = translations.SelectAtleast_OneManifest;
            $scope.SelectAtleastOneHub = translations.SelectAtleast_OneHub;
            $scope.SelectAtleastOneCustomer = translations.SelectAtleast_OneCustomer;
            $scope.ErrorWhileCreatingManifest = translations.ErrorWhileCreating_Manifest;
            $scope.Error = translations.Error;
            $scope.SUCCESS = translations.SUCCESS;
            $scope.ManifestCreatedSuccessfully = translations.Manifest_Created_Successfully;
            $scope.CreatingManifest = translations.CreatingManifest;
            $scope.LoadingNonManifestShipments = translations.LoadingNonManifestShipments;
            // GetInitial Call
        });
    };

    $scope.GetShipments = function (BagId) {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'ExpressBagShipmentsController',
            templateUrl: 'express/expressManifest/expressManifestShipment.tpl.html',
            keyboard: true,
            windowClass: 'directBookingDetail',
            size: 'lg',
            resolve: {
                BagId: function () {
                    return BagId;
                }
            }
        });
    };

    //breakbluk create tradelane shipment code here
    $scope.expressCreateShipment = function () {
        if ($scope.CustomerDetail !== undefined && $scope.CustomerDetail !== null && $scope.CustomerDetail.CustomerId > 0 &&
            $scope.CreateManifestArr !== undefined && $scope.CreateManifestArr !== null && $scope.CreateManifestArr.length > 0 &&
            $scope.Hub !== undefined && $scope.Hub !== null && $scope.Hub.HubId > 0) {
            ModalInstance = $uibModal.open({
                Animation: true,
                controller: 'ExpressCreateShipmentController',
                templateUrl: 'express/expressCreateShipment/expressCreateShipment.tpl.html',
                keyboard: true,
                windowClass: 'directBookingDetail',
                size: 'lg',
                resolve: {
                    CustomerId: function () {
                        return $scope.CustomerDetail.CustomerId;
                    },
                    Hub: function () {
                        return $scope.Hub;
                    },
                    Bags: function () {
                        return $scope.CreateManifestArr;
                    }
                }
            });
            ModalInstance.result.then(function () {
                getNonManifestedBags();
                $scope.CreateManifestArr = [];
            }, function () {
                getNonManifestedBags();
                $scope.CreateManifestArr = [];
            });
        }
        else {
            if (($scope.CreateManifestArr === undefined || $scope.CreateManifestArr === null || $scope.CreateManifestArr.length === 0) &&
                $scope.CustomerDetail !== undefined && $scope.CustomerDetail !== null && $scope.CustomerDetail.CustomerId > 0) {
                toaster.pop({
                    type: "warning",
                    title: $scope.Frayte_Warning,
                    body: $scope.SelectAtleastOneManifest,
                    showCloseButton: true
                });
            }
            else if (($scope.CustomerDetail === undefined || $scope.CustomerDetail === null || $scope.CustomerDetail.CustomerId === 0) &&
                $scope.CreateManifestArr !== undefined && $scope.CreateManifestArr !== null && $scope.CreateManifestArr.length > 0) {
                toaster.pop({
                    type: "warning",
                    title: $scope.Frayte_Warning,
                    body: $scope.SelectAtleastOneCustomer,
                    showCloseButton: true
                });
            }
                //else if (($scope.CustomerDetail === undefined || $scope.CustomerDetail === null || $scope.CustomerDetail.CustomerId === 0 ||
                //   $scope.CreateManifestArr === undefined || $scope.CreateManifestArr === null || $scope.CreateManifestArr.length === 0) &&
                //    $scope.Hub !== undefined && $scope.Hub !== null && $scope.Hub.HubId > 0) {
                //    toaster.pop({
                //        type: "warning",
                //        title: $scope.FrayteError,
                //        body: $scope.SelectAtleastOneHub,
                //        showCloseButton: true
                //    });
                //}
            else {
                toaster.pop({
                    type: "warning",
                    title: $scope.Frayte_Warning,
                    body: $scope.SelectAtleastOneManifest,
                    showCloseButton: true
                });
            }
        }
    };
    //end

    $scope.CreateManifestJson = function (Obj) {
        var isOk = false;
        if ($scope.CreateManifestArr === undefined || $scope.CreateManifestArr === null) {
            $scope.CreateManifestArr = [];
        }
        if ($scope.CreateManifestArr.length > 0) {
            for (i = 0; i < $scope.CreateManifestArr.length ; i++) {
                if ($scope.CreateManifestArr[i].BagId == Obj.BagId) {
                    isOk = true;
                }
            }
        }
        if (Obj.IsChecked === true && isOk === false) {
            $scope.CreateManifestArr.push(Obj);
        }
        else if (Obj.IsChecked === false && isOk === true) {
            for (i = 0; i < $scope.CreateManifestArr.length; i++) {
                if ($scope.CreateManifestArr[i].BagId == Obj.BagId) {
                    $scope.CreateManifestArr.splice(i, 1);
                }
            }
        }
    };

    $scope.GetBagDetails = function (Detail, _Obj) {
        if (_Obj === "Customer") {
            $scope.CustomerDetail = Detail;
        }
        else if (_Obj === "Hub") {
            $scope.Hub = Detail;
        }
        getNonManifestedBags();
    };

    var getNonManifestedBags = function () {
        //Spinner Code
        AppSpinner.showSpinnerTemplate($scope.LoadingNonManifestShipments, $scope.Template);
        ExpressManifestService.GetNonManifestedBags($scope.Hub.HubId, $scope.CustomerDetail.CustomerId).then(function (response) {
            $scope.BagData = response.data;
            for (i = 0; i < response.data.ManifestedList.length; i++) {
                response.data.ManifestedList[i].IsChecked = false;
                $scope.CreateManifestArr = [];
                response.data.ManifestedList[i].CreatedOn = DateFormatChange.DateFormatChange(response.data.ManifestedList[i].CreatedOn) + " " + response.data.ManifestedList[i].CreatedOnTime;
            }
            AppSpinner.hideSpinnerTemplate();
        }, function (response) {
            if (response.status !== 401) {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingRecord,
                    showCloseButton: true
                });
            }
        });
    };

    $scope.GetCustomers = function () {
        if ($scope.RoleId === 6 || $scope.RoleId === 1 || $scope.RoleId === 20) {
            ExpressShipmentService.GetExpressCustomers($scope.RoleId, $scope.createdBy).then(function (response) {
                if (response !== null && response.data.length > 0) {
                    $scope.ManifestCustomers = [];
                    for (var i = 0; i < response.data.length; i++) {
                        $scope.Customer = {
                            CustomerId: "",
                            CompanyName: ""
                        };
                        $scope.Customer.CustomerId = response.data[i].CustomerId;
                        $scope.Customer.CompanyName = response.data[i].CompanyName + ' - ' + accBreak(response.data[i].AccountNumber.length, response.data[i].AccountNumber);
                        $scope.ManifestCustomers.push($scope.Customer);
                    }
                    if ($scope.RoleId === 1) {
                        $scope.ManifestCustomers.unshift({ CustomerId: 0, CompanyName: 'All' });
                        $scope.CustomerDetail = $scope.ManifestCustomers[0];
                    }
                    else if ($scope.RoleId === 6) {
                        $scope.ManifestCustomers.unshift({ CustomerId: 0, CompanyName: 'All' });
                        $scope.CustomerDetail = $scope.ManifestCustomers[0];
                    }
                    else if ($scope.RoleId === 20) {
                        $scope.ManifestCustomers.unshift({ CustomerId: $scope.createdBy, CompanyName: 'All' });
                        $scope.CustomerDetail = $scope.ManifestCustomers[0];
                    }
                    if ($scope.Hub !== undefined && $scope.Hub !== null && $scope.CustomerDetail !== undefined && $scope.CustomerDetail !== null) {
                        getNonManifestedBags();
                    }
                }
            });
        }
        else {
            $scope.CustomerDetail = {};
            $scope.CustomerDetail.CustomerId = $scope.createdBy;
            if ($scope.Hub !== undefined && $scope.Hub !== null && $scope.CustomerDetail !== undefined && $scope.CustomerDetail !== null) {
                getNonManifestedBags();
            }
        }

        var accBreak = function (fiterCountryLength, AccountNo) {
            var AccNo = AccountNo.split('');
            var AccNonew = [];
            AccNonew2 = "";
            if (fiterCountryLength <= 3) {
                for (j = 0; j < fiterCountryLength; j++) {
                    if (j === 0) {
                        AccNonew.push('a');
                    }
                    AccNonew.push(AccNo[j]);
                }
                AccNonew.splice(0, 1);
                for (jj = 0; jj < AccNonew.length; jj++) {
                    AccNonew2 = AccNonew2 + AccNonew[jj].toString();
                }
            }
            else if (fiterCountryLength >= 4 && fiterCountryLength <= 8) {

                for (j = 0; j < fiterCountryLength; j++) {

                    if (j === 0) {
                        AccNonew.push('a');
                    }
                    AccNonew.push(AccNo[j]);
                }
                AccNonew.splice(0, 1);
                for (jj = 0; jj < AccNonew.length; jj++) {
                    AccNonew2 = AccNonew2 + AccNonew[jj].toString();
                }
            }
            else if (fiterCountryLength > 8) {

                for (j = 0; j < fiterCountryLength; j++) {

                    if (j === 0) {
                        AccNonew.push('a');
                    }
                    AccNonew.push(AccNo[j]);
                }
                AccNonew.splice(0, 1);
                for (jj = 0; jj < AccNonew.length; jj++) {
                    if (jj === 3) {
                        AccNonew2 = AccNonew2 + '-' + AccNonew[jj].toString();
                    }
                    else if (jj === 6) {
                        AccNonew2 = AccNonew2 + '-' + AccNonew[jj].toString();
                    }
                    else {
                        AccNonew2 = AccNonew2 + AccNonew[jj].toString();
                    }
                }
            }
            return AccNonew2;
        };
    };

    $scope.GetHubs = function () {
        ExpressManifestService.getHubs().then(function (response) {
            if (response !== null && response.data.length > 0) {
                $scope.HubList = response.data;
                $scope.Hub = {};
                $scope.Hub.HubId = 0;
                $scope.GetCustomers();
            }
        }, function () {
            toaster.pop({
                type: "error",
                title: $scope.FrayteError,
                body: $scope.ErrorGettingRecord,
                showCloseButton: true
            });
        });

    };

    function init() {
        setModalOptions();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;
        $scope.createdBy = userInfo.EmployeeId;
        $scope.GetHubs();
    }
    init();
});