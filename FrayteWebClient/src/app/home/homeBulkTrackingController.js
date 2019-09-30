angular.module('ngApp.home').controller('HomeBulkTrackingController', function (AppSpinner, $scope, $translate, $anchorScroll, $location, $state, $stateParams, config, $filter, CountryService, CourierService, HomeService, SessionService, $uibModal, $log, toaster) {
    var setModalOptions = function () {
        $translate(['Address', 'Book', 'DeleteHeader', 'DeleteBody', 'Tradelane', 'SuccessfullySavedInformation', 'FrayteInformation',
            'FrayteValidation', 'PleaseCorrectValidationErrors', 'FrayteError', 'ErrorSavingRecord', 'ErrorGetting', 'customer',
            'detail', 'TrackingDetails_Validation']).then(function (translations) {
            $scope.headerTextOtherAddress = translations.Address + " " + translations.Book + " " + translations.DeleteHeader;
            $scope.bodyTextOtherAddress = translations.DeleteBody + " " + translations.Address;
            $scope.headerTextTradeLane = translations.Tradelane + " " + translations.DeleteHeader;
            $scope.bodyTextTradeLane = translations.DeleteBody + " " + translations.Tradelane + " " + translations.detail;

            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;

            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextSavingError = translations.ErrorSavingRecord;

            $scope.TextErrorGetting = translations.ErrorGetting + " " + translations.customer + " " + translations.detail;
            $scope.TrackingDetailsValidation = translations.TrackingDetails_Validation;

        });
    };
    //state to Tracking Detail Page
    $scope.BulkTrackingJson = function () {
        $scope.BulkTracking = [{
            Carrier: null,
            TrackingCode: ""
        },
        {
            Carrier: null,
            TrackingCode: ""
        },
         {
             Carrier: null,
             TrackingCode: ""
         },
          {
              Carrier: null,
              TrackingCode: ""
          },
           {
               Carrier: null,
               TrackingCode: ""
           }
        ];
    };

    $scope.addBulkTracking = function () {
        
        
        var data =
           [{
               Carrier: null,
               TrackingCode: ""
           },
        {
            Carrier: null,
            TrackingCode: ""
        },
         {
             Carrier: null,
             TrackingCode: ""
         },
          {
              Carrier: null,
              TrackingCode: ""
          },
           {
               Carrier: null,
               TrackingCode: ""
           }
           ];
        for (i = 0 ; i < data.length ; i++) {
            
            $scope.BulkTracking.push(data[i]);
        }
        

    };


    $scope.bulkTrackingDetail = function (isValid, submitted, BulkTrackings) {
        //$scope.spinnerMessage = 'Loading Bulk Tracking...';
        
       
        if (submitted) {
            if (isValid) {
                var flag = false;
                for (var j = 0 ; j < BulkTrackings.length ; j++) {
                    if (BulkTrackings[j].Carrier === null && (BulkTrackings[j].TrackingCode === null || BulkTrackings[j].TrackingCode === "")) {
                        flag = false;
                    }
                    else {
                        flag = true;
                        break;
                    }
                }


                if (BulkTrackings !== undefined && BulkTrackings.length > 0 && flag) {
                    for (var n = 0 ; n < BulkTrackings.length; n++) {
                        BulkTrackings[n].TrackingCode = BulkTrackings[n].TrackingCode.replace(/\s/g, '');
                    }
                    AppSpinner.showSpinnerTemplate("Loading Bulk Tracking", $scope.Template);
                    HomeService.GetBulkTrackingDetails(BulkTrackings).then(function (response) {
                        AppSpinner.hideSpinnerTemplate();
                        if (response.data !== null && response.data.Tracking !== null && response.data.Tracking.length > 0) {
                            HomeService.bulkTracking = response.data.Tracking;
                            $state.go('home.bulk-trackingDetail');
                            
                        }
                    }, function () {
                        $state.go('home.tracking-error', { trackingId: 1 });
                    });
                   
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteError,
                        body: $scope.TrackingDetailsValidation,
                        showCloseButton: true
                    });
                }
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextValidation,
                    showCloseButton: true
                });
            }
        }


    };
    $scope.checkTrackingCode = function (trackingCode) {
        if (trackingCode !== undefined && trackingCode !== null && trackingCode !== '') {
            return true;
        }
        else {
            return false;
        }
    };
    $scope.checkCaarier = function (carrier) {
        if (carrier !== undefined && carrier !== null) {
            return true;
        }
        else {
            return false;
        }
    };
    var setScroll = function () {
        $location.hash('bulkTracking');
        $anchorScroll();
    };
    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $anchorScroll.yOffset = 400;
        setScroll();
        HomeService.GetCarriers().then(function (response) {
            $scope.couriers = [];
            var data = response.data;
            for (var i = 0 ; i < data.length; i++) {
                if (data[i].CourierType === "Courier") {
                    data[i].Name = data[i].Name.replace("Courier", "").trim();
                    $scope.couriers.push(data[i]);
                }
            }

        }, function () {

        });
        $scope.BulkTrackingJson();
        setModalOptions();
    }

    init();

});