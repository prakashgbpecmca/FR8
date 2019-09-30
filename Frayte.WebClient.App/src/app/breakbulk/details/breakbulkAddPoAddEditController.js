angular.module('ngApp.breakBulk').controller("BreakbulkAddPoAddEditController", function ($scope, $uibModal, ModalService, config) {


    // po shipment json code

    $scope.breakbulkShipments = [
        {
            po: '012301', styleName: 'Style 1', poCreatedOn: '23-May-2019', exFactoryDate: '29-May-2019', status: 'Delivered', styleNo: '081 8651 2493', fromTo: 'IND-CHN', createdOn: '20-May-2018', active: false, collapse: [
                { jobno: '012347', po: '2493', qty: '10', fromTo: 'IND-CHN', styleNo: '081 8651 2493', status: 'Delivered', createdOn: '22-May-2018', trackingNo: '125 256 85', courier: 'DHL Express', active: false }]
        },
        {
            po: '012302', styleName: 'Style 1', poCreatedOn: '23-May-2019', exFactoryDate: '29-May-2019', status: 'Delivered', styleNo: '081 8651 2493', fromTo: 'IND-CHN', createdOn: '25-May-2018', active: false, collapse: [
                { jobno: '012348', po: '2493', qty: '10', fromTo: 'IND-CHN', styleNo: '081 8651 2493', status: 'Delivered', createdOn: '28-May-2018', trackingNo: '125 256 85', courier: 'DHL Express', active: false },
                { jobno: '012349', po: '2493', qty: '10', fromTo: 'IND-CHN', styleNo: '081 8651 2493', status: 'Delivered', createdOn: '29-May-2018', trackingNo: '125 256 85', courier: 'DHL Express', active: false }
            ]
        },
        {
            po: '012303', styleName: 'Style 1', poCreatedOn: '23-May-2019', exFactoryDate: '29-May-2019', status: 'Delivered', styleNo: '081 8651 2494', fromTo: 'IND-CHN', createdOn: '25-May-2018', active: false, collapse: [
                { jobno: '012350', po: '2493', qty: '10', fromTo: 'IND-CHN', styleNo: '081 8651 2493', status: 'Delivered', createdOn: '28-May-2018', trackingNo: '125 256 85', courier: 'DHL Express', active: false },
                { jobno: '012351', po: '2493', qty: '10', fromTo: 'IND-CHN', styleNo: '081 8651 2493', status: 'Delivered', createdOn: '29-May-2018', trackingNo: '125 256 85', courier: 'DHL Express', active: false }
            ]
        },
        {
            po: '012304', styleName: 'Style 1', poCreatedOn: '23-May-2019', exFactoryDate: '29-May-2019', status: 'Delivered', styleNo: '081 8651 2495', fromTo: 'IND-CHN', createdOn: '25-May-2018', active: false, collapse: [
                { jobno: '012352', po: '2493', qty: '10', fromTo: 'IND-CHN', styleNo: '081 8651 2493', status: 'Delivered', createdOn: '28-May-2018', trackingNo: '125 256 85', courier: 'DHL Express', active: false },
                { jobno: '012353', po: '2493', qty: '10', fromTo: 'IND-CHN', styleNo: '081 8651 2493', status: 'Delivered', createdOn: '29-May-2018', trackingNo: '125 256 85', courier: 'DHL Express', active: false }
            ]
        },
        {
            po: '012305', styleName: 'Style 1', poCreatedOn: '23-May-2019', exFactoryDate: '29-May-2019', status: 'Delivered', styleNo: '081 8651 2496', fromTo: 'IND-CHN', createdOn: '25-May-2018', active: false, collapse: [
                { jobno: '012354', po: '2493', qty: '10', fromTo: 'IND-CHN', styleNo: '081 8651 2493', status: 'Delivered', createdOn: '28-May-2018', trackingNo: '125 256 85', courier: 'DHL Express', active: false },
                { jobno: '012355', po: '2493', qty: '10', fromTo: 'IND-CHN', styleNo: '081 8651 2493', status: 'Delivered', createdOn: '29-May-2018', trackingNo: '125 256 85', courier: 'DHL Express', active: false }
            ]
        }
    ];

    //end



    //checkbox check code

    $scope.CheckUncheckHeader = function () {
        $scope.IsAllChecked = true;
        for (var i = 0; i < $scope.breakbulkShipments.length; i++) {
            if (!$scope.breakbulkShipments[i].active) {
                $scope.IsAllChecked = false;
                break;
            }
        }
    };
    $scope.CheckUncheckHeader();

    $scope.CheckUncheckAll = function () {
        for (var i = 0; i < $scope.breakbulkShipments.length; i++) {
            $scope.breakbulkShipments[i].active = $scope.IsAllChecked;
        }
    };

    //end



    function init() { }
    init();



});