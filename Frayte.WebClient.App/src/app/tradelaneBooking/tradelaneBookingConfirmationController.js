angular.module('ngApp.tradelaneBooking').controller('HAWBConfirmationController', function ($scope, ModalService, TotalShipments, AllocatedShipments,Type, $translate, $uibModalInstance) {
  
        $scope.closePopUp = function () {
        $uibModalInstance.close();
      
    };    
   
    function init() {
       
        if (Type) {
            $scope.type = Type;
        }
        else {
            $scope.type = "";
        }
        $scope.TotalShipments = TotalShipments;
        $scope.AllocatedShipments = AllocatedShipments;
         
    }

    init();

});