angular.module("ngApp.previewMAWB")
.controller("PreviewMAWBAddressController", function ($scope, $state, PreviewMAWBService, TradelaneBookingService, TopCountryService, $translate, Address, $uibModalInstance, toaster, ModalService, SessionService, AppSpinner, DirectBookingService, DirectShipmentService, $rootScope, TradelaneShipmentService, DateFormatChange) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'Frayte-Error', 'FrayteWarning', 'Error_In_Updating_Address', 'Please_Correct_Validation_Error']).then(function (translations) {

            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteError = translations.FrayteError;
            $scope.Error_In_Updating_Address = translations.Error_In_Updating_Address;
            $scope.Please_Correct_Validation_Error = translations.Please_Correct_Validation_Error;

        });
    };
    //end
    
    $scope.editAddress = function (Type) {
        if (Type) {
            ModalInstance = $uibModal.open({
                Animation: true,
                templateUrl: 'previewMAWB/mawb.tpl.html',
                controller: 'PreviewMAWBController',
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
        }
    };

     
        $scope.shipFromToggleState = function (Country) {
            return TradelaneBookingService.toggleState(Country);
        };
     

    $scope.changeAddress = function (IsValid) {
        if (IsValid) {
            console.log($scope.address); 
        PreviewMAWBService.ChangeAddress($scope.address).then(function (response) { 
            $uibModalInstance.close(); 
          
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.Error_In_Updating_Address,
                showCloseButton: true
            });
           
        });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.Please_Correct_Validation_Error,
                showCloseButton: true
            });
        }
   };


   var setStatePostCodeForHKGUK = function (Country, Type) {
       if (Country.Code === 'HKG') {
           if (Type === 'Shipper') {
               $scope.address.PostCode = null;
               $scope.address.State = null;
           }
       }
       else if (Country.Code === 'GBR') {
           if (Type === 'Shipper') {
               $scope.address.State = null;
           }
       }
   };
   $scope.SetShipinfo = function (Country, Action) {
       if (Country) {
           if (Action === 'Shipper') {
             
               $scope.showPostCodeDropDown = false;
               for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
                   if ($scope.CountryPhoneCodes[i].CountryCode === Country.Code) {
                       $scope.ShipFromPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                       break;
                   }
               }
           } 
           setStatePostCodeForHKGUK(Country, Action);
       }
   };

   var screenInitials = function () {
       TradelaneBookingService.BookingInitials($scope.userInfo.EmployeeId).then(function (response) {
       
           // Set Country type according to given order
           $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries); 

           $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;
           $scope.SetShipinfo($scope.address.Country, "Shipper");
     
       },
   function (response) {
      
       
   });
   };
    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.userInfo = SessionService.getUser();
        setModalOptions();
        if ($scope.userInfo) {
            if (Address) {
                $scope.address = Address;
            }
            screenInitials();
        }
        else {
            $state.go("login");
        }
    }

    init();
});