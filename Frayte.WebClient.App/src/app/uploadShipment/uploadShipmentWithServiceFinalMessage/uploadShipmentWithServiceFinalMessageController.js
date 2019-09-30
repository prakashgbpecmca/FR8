angular.module('ngApp.uploadShipment').controller('WithServiceFinalMessageController', function ($scope, $uibModal, BatchProcessedShipments, BatchUnprocessedShipments, BatchProcess, config, SessionRecord) {
    
  



    function init() {
        $scope.UnsuccessFullShipments = [];
        $scope.ImagePath = config.BUILD_URL;
        $scope.Total = BatchProcess;
        $scope.Processed = BatchProcessedShipments;
        $scope.Unprocessed = BatchUnprocessedShipments;
        $scope.SessionName = SessionRecord.SessionName;
        
    }

    init();

});