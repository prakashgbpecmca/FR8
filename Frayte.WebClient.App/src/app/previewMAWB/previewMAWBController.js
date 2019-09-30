angular.module("ngApp.previewMAWB")
.controller("PreviewMAWBController", function ($scope, $state, PreviewMAWBService, $translate, ShipmentId, $uibModal, toaster, ModalService, SessionService, AppSpinner, DirectBookingService, DirectShipmentService, $rootScope, TradelaneShipmentService, DateFormatChange) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'Frayte-Error', 'FrayteWarning', 'Error_In_getting_MAWB_Details', 'Getting_MAWB_detail']).then(function (translations) {

            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteError = translations.FrayteError;
            $scope.Error_In_getting_MAWB_Details = translations.Error_In_getting_MAWB_Details;
            $scope.Getting_MAWB_detail = translations.Getting_MAWB_detail;

        });
    };
    //end

    $scope.editAddress = function (Type) {
        if (Type) { 
            ModalInstance = $uibModal.open({
                Animation: true,
                templateUrl: 'previewMAWB/addressAddEdit.tpl.html',
                controller: 'PreviewMAWBAddressController',
                keyboard: true,
                windowClass: 'DirectBookingDetail',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    Address: function () {
                        if (Type === "Shipper") {
                            return $scope.mAwbDetail.ShipmentDetail.ShipFrom;
                        }
                        else {
                            return $scope.mAwbDetail.ShipmentDetail.ShipTo;
                        }                  
                    }
                }
            });
            ModalInstance.result.then(function (response) {
                getScreenInitials();
            }, function () {
                //     $state.go("loginView.userTabs.tradelane-shipments");
            });
        }
    };


    var getScreenInitials = function () {

        AppSpinner.showSpinnerTemplate($scope.Getting_MAWB_detail, $scope.Template);

      PreviewMAWBService.MAWBPreview($scope.shipmentId).then(function (response) {

          if (response.data) {
              $scope.mAwbDetail = response.data;
          }

          AppSpinner.hideSpinnerTemplate();
      }, function () {
          toaster.pop({
              type: 'error',
              title: $scope.FrayteError,
              body: $scope.Error_In_getting_MAWB_Details,
              showCloseButton: true
          });
          AppSpinner.hideSpinnerTemplate();
      }); 

    };


    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.userInfo = SessionService.getUser();
        setModalOptions();
        if ($scope.userInfo) {
            if (ShipmentId) { 
                $scope.shipmentId = ShipmentId;
                getScreenInitials();
            }
        }
        else {
            $state.go("login");
        }
    }

    init();
});