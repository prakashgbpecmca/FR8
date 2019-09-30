angular.module('ngApp.tradelane').controller('TradelaneAddEditController', function ($uibModalStack, $scope, $state, $location, $filter, $translate, SessionService, $uibModal, TradelaneService, toaster, uiGridConstants, mode, tradelane, tradelanes, countries, $uibModalInstance, AgentService, CarrierService, ModalService) {

    var setModalOptions = function () {
        $translate(['FrayteValidation', 'PleaseCorrectValidationErrors', 'Cancel_Validation', 'Cancel', 'Confirmation']).then(function (translations) {
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.CancelConfirmation = translations.Cancel + " " + translations.Confirmation;
            $scope.CancelValidation = translations.Cancel_Validation;
        });
    };

    $scope.GetCountryAgents = function (country, selectionType) {
        AgentService.GetCountryAgents(country.CountryId).then(function (response) {
            if (selectionType === 'Originating') {
                $scope.originatingAgents = response.data;
                $scope.restoreOriginatingAgents = response.data;
            }
            else if (selectionType === 'Destination') {
                $scope.destinationAgents = response.data;
                $scope.restoreDestinationAgents = response.data;
            }
            if ($scope.tradelaneDetail.CarrierType !== undefined && $scope.tradelaneDetail.CarrierType !== null && $scope.tradelaneDetail.CarrierType !== '') {
                $scope.GetCarrierTypes($scope.tradelaneDetail.CarrierType);
            }
        });
    };

    $scope.GetCountryAgentsAtLoad = function () {
        if ($scope.tradelaneDetail.OriginatingCountry !== undefined &&
            $scope.tradelaneDetail.OriginatingCountry !== null &&
            $scope.tradelaneDetail.OriginatingCountry.CountryId > 0) {
            AgentService.GetCountryAgents($scope.tradelaneDetail.OriginatingCountry.CountryId).then(function (response) {
                $scope.originatingAgents = response.data;

                if ($scope.tradelaneDetail.DestinationCountry !== undefined &&
                    $scope.tradelaneDetail.DestinationCountry !== null &&
                    $scope.tradelaneDetail.DestinationCountry.CountryId > 0) {
                    AgentService.GetCountryAgents($scope.tradelaneDetail.DestinationCountry.CountryId).then(function (response) {
                        $scope.destinationAgents = response.data;

                        //Finally load the carrier types
                        CarrierService.GetCarrierTypeList($scope.tradelaneDetail.CarrierType).then(function (response) {
                            $scope.carriers = response.data;
                        });
                    });
                }
            });
        }
    };

    $scope.GetCarrierTypes = function (shipmentVia) {
        var destinationArray = $scope.restoreDestinationAgents;
        var originatingArray = $scope.restoreOriginatingAgents;
        var newDestinationAgentArray = [];
        var newOriginatingAgentArray = [];
        if (destinationArray !== undefined && destinationArray !== null && destinationArray.length > 0) {
            for (var i = 0; i < destinationArray.length; i++) {
                if (shipmentVia === "Air") {
                    if (destinationArray[i].IsAir) {
                        newDestinationAgentArray.push(destinationArray[i]);
                    }
                }
                if (shipmentVia === "Sea") {
                    if (destinationArray[i].IsSea) {
                        newDestinationAgentArray.push(destinationArray[i]);
                    }
                }
            }
            $scope.destinationAgents = newDestinationAgentArray;
        }
        if (originatingArray !== undefined && originatingArray !== null && originatingArray.length > 0) {
            for (var j = 0; j< originatingArray.length; j++) {
                if (shipmentVia === "Air") {
                    if (originatingArray[j].IsAir) {
                        newOriginatingAgentArray.push(originatingArray[j]);
                    }
                }
                if (shipmentVia === "Sea") {
                    if (originatingArray[j].IsSea) {
                        newOriginatingAgentArray.push(originatingArray[j]);
                    }
                }

            }
            $scope.originatingAgents = newOriginatingAgentArray;

        }


        CarrierService.GetCarrierTypeList(shipmentVia).then(function (response) {
            $scope.carriers = response.data;
        });
    };

    $scope.UpdateTradelane = function (tradelaneDetail) {
        var objects = $scope.tradelanes;

        for (var i = 0; i < objects.length; i++) {
            if (objects[i].SN === tradelaneDetail.SN) {
                objects[i] = tradelaneDetail;
                break;
            }
        }
    };

    $scope.SaveTradelane = function (isValid, tradelaneDetail) {
        if (isValid) {
            tradelaneDetail.Direct = false;
            tradelaneDetail.Deffered = false;
            if (tradelaneDetail.DirectDefferedType === 'Direct') {
                tradelaneDetail.Direct = true;
            }
            else if (tradelaneDetail.DirectDefferedType === 'Deffered') {
                tradelaneDetail.Deffered = true;
            }

            if (tradelaneDetail.SN === undefined || tradelaneDetail.SN === 0) {
                tradelaneDetail.SN = $scope.tradelanes.length + 1;
                $scope.tradelanes.push(tradelaneDetail);
            }
            else {
                //Need to update the tradelane collection and then return back to main grid
                $scope.UpdateTradelane(tradelaneDetail);
            }

            $uibModalInstance.close($scope.tradelanes);
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

    $scope.TradlaneGoBack = function () {
        var modalOptions = {
            headerText: $scope.CancelConfirmation,
            bodyText: $scope.CancelValidation
        };

        ModalService.Confirm({}, modalOptions).then(function () {
            //$state.go('dbuser.customer-detail.basic-detail');
            $uibModalStack.dismissAll('ok');
        }, function () {


        });



        //modalInstance = $uibModal.open({
        //    animation: true,
        //    templateUrl: 'customer/customerDetail/customerBasicDetail/cancelConfirmation.tpl.html',
        //    size: 'md',
        //    keyboard: false,
        //    backdrop: 'static'

        //});
        //modalInstance.result.then(function () {
        //    $state.go('dbuser.customer-detail.basic-detail', {}, { reload: true });
        //}, function () { });
    };

    function init() {
        //Multilingual Modal Option
        setModalOptions();

        //$scope.mode = mode;
        if (mode === "Add") {
            $translate('Add').then(function (add) {
                $scope.mode = add;
            });
        }
        if (mode === "Modify") {
            $translate('Modify').then(function (modify) {
                $scope.mode = modify;
            });
        }

        //Set initial values
        $scope.tradelaneDetail = tradelane;
        $scope.tradelanes = tradelanes;

        $scope.countries = countries;

        //Set DirectDefferedType
        if ($scope.tradelaneDetail.Direct) {
            $scope.tradelaneDetail.DirectDefferedType = 'Direct';
        }
        else if ($scope.tradelaneDetail.Deffered) {
            $scope.tradelaneDetail.DirectDefferedType = 'Deffered';
        }

        $scope.GetCountryAgentsAtLoad();
    }

    init();

});