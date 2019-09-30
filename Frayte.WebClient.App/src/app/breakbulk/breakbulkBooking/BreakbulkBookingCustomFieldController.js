angular.module('ngApp.breakBulk').controller("BreakbulkBookingCustomFieldController", function ($scope, $uibModal, ModalService,  $uibModalInstance, config, BreakBulkService) {

    $scope.customField = 'Custom Field';

    $scope.customTypes = [
        { id: 1, name: 'Text', active: false },
        { id: 2, name: 'Dropdown', active: false }
    ];

    $scope.customSubmit = function (customname, customNumber) {
        if (customname !== null && customname !== undefined && customname !== '' && customNumber !== null &&  customNumber !== '') {
            $scope.customTypes.active = true;
            $scope.obj = {
                customField1 : customNumber,
                customField1Name : customname
            };

          

            $uibModalInstance.close($scope.obj);
        }
    };


    //$scope.cancel = function () {
    //    //if (customname !== null && customname !== undefined && customname !== '' && customNumber !== null && customNumber !== undefined && customNumber !== '') {
    //        $uibModalInstance.dismiss('cancel');
    //    //}
    //}; 

    $uibModalInstance.result.then(function cancel() {
        $uibModalInstance.close('cancel');
    },
        function errorCallBack() {
            $uibModalInstance.close();
        });  

    function init() { }
    init();

});