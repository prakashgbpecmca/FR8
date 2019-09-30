angular.module('ngApp.customer').controller('CustomerController', function ($rootScope, $scope,UtilityService, $state, Upload, config, $location, $filter, $translate, CustomerService, ShipmentService, SessionService, $uibModal, uiGridConstants, toaster, $window, AppSpinner, ModalService) {
    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['Address', 'Book', 'DeleteHeader', 'DeleteBody', 'Tradelane', 'SuccessfullySavedInformation', 'FrayteInformation', 'FrayteValidation',
            'PleaseCorrectValidationErrors', 'FrayteError', 'ErrorSavingRecord', 'ErrorGetting', 'Customer', 'detail', 'Zone',
            'Shipment_Type', 'Select_CourierAccount', 'CustomerDelete_Validation', 'CustomerDeleteError_Validation', 'Delete', 'Confirmation', 'ACustomer_Record']).then(function (translations) {
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
                $scope.CustomerDeleteHeader = translations.Customer + " " + translations.Delete + " " + translations.Confirmation;
                $scope.CustomerDelete = translations.DeleteBody + " " + translations.ACustomer_Record;

                $scope.GettingZoneError = translations.ErrorGetting + " " + translations.Zone;
                $scope.TextErrorGettingShipment = translations.ErrorGetting + " " + translations.customer + " " + translations.Shipment_Type;
                $scope.SelectCourierAccount = translations.Select_CourierAccount;
                $scope.CustomerDeleteValidation = translations.CustomerDelete_Validation;
                $scope.CustomerDeleteErrorValidation = translations.CustomerDeleteError_Validation;
            });
    };

    // Uplaod agnets via excel
    $scope.WhileAddingReceiverExcel = function ($files, $file, $event) {
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        // Upload the excel file here.
        $scope.uploadExcel = Upload.upload({

            url: config.SERVICE_URL + '/Customer/UploadCustomers',
            file: $file

        });

        $scope.uploadExcel.progress($scope.progressExcel);

        $scope.uploadExcel.success($scope.successExcel);

        $scope.uploadExcel.error($scope.errorExcel);
    };

    $scope.progressExcel = function (evt) {
        //console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
        //toaster.clear();
        //toaster.pop({
        //    type: 'success',
        //    title: 'uploading',
        //    body: 'percent: ' + parseInt(100.0 * evt.loaded / evt.total, 10),
        //    showCloseButton: true
        //});
    };

    $scope.successExcel = function (data, status, headers, config) {
        if (status = 200) {

            toaster.pop({
                type: 'success',
                title: $scope.TitleFrayteSuccess,
                body: $scope.TextUploadedSuccessfully,
                showCloseButton: true
            });
            $state.reload();
        }

    };

    $scope.errorExcel = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scope.TextErrorOccuredDuringUpload,
            showCloseButton: true
        });
    };

    $scope.DeleteCustomer = function (customer) {
        var modalOptions = {
            headerText: $scope.CustomerDeleteHeader,
            
            bodyText: $scope.CustomerDelete
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            CustomerService.DeleteCustomer(customer.UserId).then(function () {
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteSuccess,
                    body: $scope.CustomerDeleteValidation,
                    showCloseButton: true,
                    closeHtml: '<button></button>'
                });
                $scope.LoadCustomers();
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.CustomerDeleteErrorValidation,
                    showCloseButton: true,
                    closeHtml: '<button>Close</button>'
                });
            });
        });
       
    };

    $scope.AddEditCustomer = function (row) {  
        if ($scope.tab !== undefined && $scope.tab !== null && $scope.tab.childTabs !== null) {
            var route = UtilityService.GetCurrentRoute($scope.tab.childTabs, "customer-detail.basic-detail");
            if (row === undefined) {
                $state.go(route, { "customerId": 0 });
            }
            else {
                $state.go(route, { "customerId": row.entity.UserId });
            }
           
        }
    };

    //$scope.isActive = function (item) {
    //    
    //    return true;
    //};
    $scope.SetCustomerGridOptions = function () {
        $scope.gridOptions = {
            showFooter: true,
            enableSorting: true,
            multiSelect: false,
            enableFiltering: true,
            enableRowSelection: true,
            enableSelectAll: false,
            enableRowHeaderSelection: false,
            selectionRowHeaderWidth: 35,
            noUnselect: true,
            enableGridMenu: true,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            columnDefs: [
                { name: 'CustomerAccountNo', displayName: "Frayte_Account", headerCellFilter: 'translate', width: '15%' },
              { name: 'CompanyName', displayName: "Company", headerCellFilter: 'translate', width: '15%' },
              { name: 'ContactName', displayName: "Contact_Name", headerCellFilter: 'translate', width: '15%' },
              { name: 'UserAddress.Country.Name', displayName: "Country", headerCellFilter: 'translate', width: '11%' },
              { name: 'Email', headerCellFilter: 'translate', width: '21%' },
              { name: 'TelephoneNo', displayName: "Phone_No", headerCellFilter: 'translate', width: '13%' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "customer/customerEditButton.tpl.html" }
            ]
        };
    };

    $scope.LoadCustomers = function () {
        AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
        CustomerService.GetCustomerList($scope.UserId).then(function (response) { 
            shipMentdtl(response.data);
            var UIdata = response.data;
            for (i = 0; i < response.data.length; i++) {
                var newdata = "";
                var a = UIdata[i].CustomerAccountNo.split('');
                for (j = 0; j < a.length; j++) {
                    newdata = newdata + a[j];
                    if (j === 2 || j === 5) {
                        newdata = newdata + "-";
                    }

                }
                response.data[i].CustomerAccountNo = newdata;
            }

         
            $scope.gridOptions.data = response.data;
            //For full List/Data in UI-GRID
            $scope.gridOptions.excessRows = $scope.gridOptions.data.length;
         
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    };
    function shipMentdtl(data) { 
        ShipmentService.GetInitials().then(function (response) {
            $scope.countryPhoneCodes = response.data;
            for (k = 0; k < data.length; k++) {
                for (l = 0; l < response.data.CountryPhoneCodes.length ; l++) {
                    if (data[k].UserAddress.Country.Code === response.data.CountryPhoneCodes[l].CountryCode) {
                        data[k].TelephoneNo = "(+" + response.data.CountryPhoneCodes[l].PhoneCode +")" + " " + data[k].TelephoneNo;
                    }
                }
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    }
    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
    };
    var getUserTab = function (tabs, tabKey) {
        if (tabs !== undefined && tabs !== null && tabs.length) {
            var tab = {};
            for (var i = 0; i < tabs.length; i++) {
                if (tabs[i].tabKey === tabKey) {
                    tab = tabs[i];
                    break;
                }
            }
            return tab;
        }
    };
    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.spinnerMessage = 'Loading Customers';
        var userInfo = SessionService.getUser();
        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "Customers");
        $scope.RoleId = userInfo.RoleId;
        $scope.UserId = userInfo.EmployeeId;
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetCustomerGridOptions();
        $scope.LoadCustomers();
        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };
        setModalOptions();
        $rootScope.GetServiceValue = null;
    }

    init();

});