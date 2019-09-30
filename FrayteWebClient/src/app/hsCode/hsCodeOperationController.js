///**
// * Controller
// */
//angular.module('ngApp.hsCode').controller('HSCodeOperationController', function ($scope,$rootScope, DirectShipmentService ,$location,config , $filter, AppSpinner, $translate, SessionService, UserService, $uibModal, toaster, ShipmentService, $log, $state, $stateParams, ModalService, TopCountryService) {

//    $scope.SeachDirectShipments = function () {
//        $scope.DirectShipments();
//    };
//    var statusTypeTemplate = '<div class="ui-grid-cell-contents"><img ng-src="{{grid.appScope.buildURL}}{{grid.appScope.currentImgURL(row)}}" style="width:40px;margin: -5px 0px 0px;position:relative;">{{grid.appScope.GetStatus(row)}}</div>';
//    $scope.currentImgURL = function (row) {
//        if (row !== undefined) {
//            var url = '';
//            for (var i = 0; i < $scope.DirectShipmentStatus.length; i++) {
//                if (row.entity.Status == $scope.DirectShipmentStatus[i].StatusName) {
//                    url = $scope.DirectShipmentStatus[i].ImgURL;
//                    break;
//                }
//            }
//            return url;
//        }
//    };

//    $scope.GetStatus = function (row) {
//        if (row !== undefined) {
//            return row.entity.DisplayStatus;
//        }
//    };
//    $scope.rowFormatter = function (row) {
//        return true;
//    };

//    var trackingTemplate = '<div><a target="_blank" ui-sref="home.tracking-hub({carrierType:row.entity.CourierName,trackingId:row.entity.TrackingCode, RateType: row.entity.RateType})" style="top:5px;position:relative;">{{row.entity.TrackingNo}}</a></div>';
//    $scope.SetGridOptions = function () {
//        $scope.gridOptions = {
//            showFooter: true,
//            enableSorting: true,
//            multiSelect: false,
//            enableFiltering: true,
//            enableRowSelection: true,
//            enableSelectAll: false,
//            enableRowHeaderSelection: false,
//            selectionRowHeaderWidth: 35,
//            noUnselect: true,
//            enableGridMenu: true,
//            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
//            enableVerticalScrollbar: true,
//            columnDefs: [
//                  { name: 'DisplayStatus', displayName: 'Status', width: '13%', enableFiltering: false, enableSorting: false, headerCellFilter: 'translate', cellTemplate: statusTypeTemplate },
//                  { name: 'DisplayName', displayName: 'ShipmentMethod', width: '15%', headerCellFilter: 'translate' },
//                  { name: 'TrackingNo', displayName: 'Tracking_No', cellTemplate: trackingTemplate, width: '12%', headerCellFilter: 'translate' },
//                  { name: 'FrayteNumber', displayName: 'Frayte_ShipmentNo', width: '13%', headerCellFilter: 'translate' },
//                  { name: 'ShippedFromCompany', displayName: 'From_Shipper', width: '20%', headerCellFilter: 'translate' },
//                  { name: 'ShippedToCompany', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '17%' },
//                  { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "directBooking/directShipments/directShipmentEditButton.tpl.html" }
//            ]
//        };
//    };

//    $scope.buildURL = config.BUILD_URL;

//    $scope.toggleRowSelection = function () {
//        $scope.gridApi.selection.clearSelectedRows();
//        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
//        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
//    };

//    $scope.LoadShipmentsWithoutHSCode = function () {

//        HSCodeService.ShipmentWithoutHSCodes($scope.obj).then(function (response) {
//            if(response.data === null){
//                $scope.gridOptions.data = response.dat;
//            }
//            else {
//                toaster.pop({
//                    type: 'error',
//                    title: "Frayte-Error",
//                    body: "Error While geting records.",
//                    showCloseButton: true
//                });
//            }
//        }, function () {
//            toaster.pop({
//                type: 'error',
//                title: "Frayte-Error",
//                body: "Error While geting records.",
//                showCloseButton: true
//            });
//        });

//    };

//    $scope.LoadShipmentStatus = function () {
//        //  getUserId();
//        DirectShipmentService.GetDirectShipmentStatus("eCommerce", $scope.UserId).then(function (response) {
//            $scope.ShipmentStatus = response.data;

//            for (var i = 0 ; i < $scope.ShipmentStatus.length; i++) {
//                if ($scope.ShipmentStatus[i].StatusName === "Current") {
//                    $scope.ShipmentStatus[i].ImgURL = "currentImg.png";
//                }
//                else if ($scope.ShipmentStatus[i].StatusName === "Draft") {
//                    $scope.ShipmentStatus[i].ImgURL = "draftImg.png";
//                }
//                else if ($scope.ShipmentStatus[i].StatusName === "Past") {
//                    $scope.ShipmentStatus[i].ImgURL = "pastImg.png";
//                }
//                else if ($scope.ShipmentStatus[i].StatusName === "Delay") {
//                    $scope.ShipmentStatus[i].ImgURL = "delayImg.png";
//                }
//                else if ($scope.ShipmentStatus[i].StatusName === "Cancel") {
//                    $scope.ShipmentStatus[i].ImgURL = "cancelImg.png";
//                }
//            }

//            $scope.CurrentStatus = $scope.ShipmentStatus[0];
//            $scope.track.ShipmentStatusId = $scope.CurrentStatus.ShipmentStatusId;
//            $scope.LoadShipmentsWithoutHSCode();
//        }, function () {
//            toaster.pop({
//                type: 'error',
//                title: $scope.Frayte_Error,
//                body: $scope.GettingDataErrorValidation,
//                showCloseButton: true
//            });
//        });

//    };

//    function init() {
//        var userInfo = SessionService.getUser();
//        $scope.UserId = userInfo.EmployeeId;
//        $scope.obj = {
//            ModuleType: 'eCommerce',
//            FromDate: '',
//            ToDate: '',
//            ShipmentStatusId: 17,
//            FrayteNumber: '',
//            TrackingNo: '',
//            CurrentPage: '',
//            TakeRows: '',
//            OperationZoneId: 2
//        };


//        $rootScope.GetServiceValue = null;
//        $scope.LoadShipmentStatus();
//    }

//    init();

//});