angular.module('ngApp.AddressBook').controller('CustomerAddressBookController', ['$scope', 'AppSpinner', 'DirectBookingService', '$uibModal', 'SessionService', 'ModalService', 'TopCountryService', 'toaster', 'ShipmentService', '$rootScope', '$translate', 'uiGridConstants', 'Upload', 'config', function ($scope, AppSpinner, DirectBookingService, $uibModal, SessionService, ModalService, TopCountryService, toaster, ShipmentService, $rootScope, $translate, uiGridConstants, Upload, config) {

    $scope.TrackSearchText = {
        CustomerId: null,
        SearchText: '',
        SearchBy: '',
        AddressType: 'All',
        TakeRows: null,
        CurrentPage: null
    };

    var removeDefaultAddressFromGrid = function () {
        for (i = 0; i < $scope.gridDirectBookingdata.length; i++) {
            if ($scope.gridDirectBookingdata[i].IsDefault) {
                $scope.gridDirectBookingdata[i].IsDefault = false;
                break;
            }
        }
    };

    $scope.setDefaultAddressGrid = function (row) {

        var AddressType = "";
        if (row.FromAddress) {
            AddressType = "FromAddress";
        }
        if (row.ToAddress) {
            AddressType = "ToAddress";
        }

        DirectBookingService.CustomerDefaultAddress(row.AddressbookId, row.Country.CountryId, $scope.TrackSearchText.CustomerId, AddressType).then(function (response) {
            if (response.data.Status) {
                //   removeDefaultAddressFromGrid();
                row.IsDefault = true;
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: "Successfully added as default address",
                    showCloseButton: true
                });
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: "Some error has occured.",
                    showCloseButton: true
                });
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: "Some error has occured.",
                showCloseButton: true
            });
        });

    };

    var removeDefaultAddressFromCard = function () {
        for (i = 0; i < $scope.AddressDetail.length; i++) {
            if ($scope.AddressDetail[i].IsDefault) {
                $scope.AddressDetail[i].IsDefault = false;
            }
        }
    };

    $scope.setDefaultAddressCard = function (row) {

        var AddressType = "";
        if (row.FromAddress) {
            AddressType = "FromAddress";
        }
        if (row.ToAddress) {
            AddressType = "ToAddress";
        }

        DirectBookingService.CustomerDefaultAddressBook(row.AddressbookId, row.Country.CountryId, $scope.TrackSearchText.CustomerId, row.IsDefault, AddressType).then(function (response) {
            var flag = row.IsDefault;
            if (response.data.Status) {
                //   removeDefaultAddressFromCard();
                if (flag) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: "Successfully added as default address",
                        showCloseButton: true
                    });
                    row.IsDefault = true;
                }
                else {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: "Successfully removed default address",
                        showCloseButton: true
                    });
                    row.IsDefault = false;
                }

            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: "Some error has occured.",
                    showCloseButton: true
                });
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: "Some error has occured.",
                showCloseButton: true
            });
        });

    };

    $scope.selectAdressRecord = function (gridBooking) {
        if ($scope.gridDirectBookingdata !== undefined && $scope.gridDirectBookingdata !== null && $scope.gridDirectBookingdata.length > 0) {
            for (var i = 0; i < $scope.gridDirectBookingdata.length; i++) {
                $scope.gridDirectBookingdata[i].IsSelected = false;
            }
        }
        if (gridBooking !== null && gridBooking !== undefined) {
            gridBooking.IsSelected = true;
        }
    };

    var setModalOptions = function () {
        $translate(['Frayte_Error', 'FrayteWarning', 'FrayteSuccess', 'Erase_AllAddress', 'Delete_Confirmation', 'Delete_Address',
                    'Successfull', 'FrayteError_Validation', 'AddressBook_Erased_Successfully', 'Error_While_Deleting_The_Records',
                    'Address_Deleted_Successfully', 'FrayteWarning_Validation', 'Enter_Search_Value', 'LoadingAddressBook',
                    'Already_Exist_Or_Save_Other_Address', 'Upload_AddressBookMessage', 'WeUploadingYourAddressBook']).then(function (translations) {
                        $scope.Header_Delete = translations.Delete_Confirmation;
                        $scope.Erase_Address = translations.Erase_AllAddress;
                        $scope.Deleted_Address = translations.Delete_Address;
                        $scope.Frayte_Success = translations.FrayteSuccess;
                        $scope.FrayteWarning = translations.FrayteWarning;
                        $scope.FrayteError = translations.Frayte_Error;
                        $scope.AddressBookErasedSuccessfully = translations.AddressBook_Erased_Successfully;
                        $scope.ErrorWhileDeletingTheRecords = translations.Error_While_Deleting_The_Records;
                        $scope.AddressDeletedSuccessfully = translations.Address_Deleted_Successfully;
                        $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
                        $scope.EnterSearchValue = translations.Enter_Search_Value;
                        $scope.LoadingAddressBook = translations.LoadingAddressBook;
                        $scope.Duplicate_Address_Validation = translations.Already_Exist_Or_Save_Other_Address;
                        $scope.Upload_Address_Message = translations.Upload_AddressBookMessage;
                        $scope.WeUploading_YourAddressBook = translations.WeUploadingYourAddressBook;

                        $rootScope.getAddressBook();
                    });
    };

    var clientNameTemplate = '<div class="ui-grid-cell-contents">{{grid.appScope.GetClientName(row)}}</div>';

    $scope.SetIsFavourite = function (row) {
        DirectBookingService.MarkAddressAsFavourite(row.AddressbookId, row.IsFavorites).then(function (response) {
            if (response.status === 200) {
                //$scope.IsFavouriteData = response.data;
                $rootScope.getAddressBook();
            }
        });
    };

    var setGridOptions = function () {
        $scope.gridDirectBooking = {
            showFooter: true,
            enableSorting: true,
            multiSelect: false,
            enableFiltering: true,
            enableSelectAll: false,
            selectionRowHeaderWidth: 35,
            noUnselect: true,
            enableGridMenu: false,
            enableRowSelection: true,
            enableRowHeaderSelection: false,
            enableColumnMenus: false,
            modifierKeysToMultiSelect: false,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            rowTemplate: "<div ng-dblclick=\"grid.appScope.showInfo(row)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>",
            columnDefs: [
                { name: 'IsFavorites', displayName: 'Favourite', headerCellFilter: 'translate', cellTemplate: 'directBooking/directbookingAddressBookStar.tpl.html', enableFiltering: false, width: "8%" },
                { name: 'CompanyName', displayName: 'Company', headerCellFilter: 'translate', width: "15%" },
                { name: 'CustomerName', displayName: 'Client_Name', headerCellFilter: 'translate', width: "15%" },
                { name: 'Address', displayName: 'Address_1', headerCellFilter: 'translate', width: "20%" },
                //{ name: 'Address2', displayName: 'Address_2', headerCellFilter: 'translate', width: "20%" },
                { name: 'City', displayName: 'City', headerCellFilter: 'translate', width: '10%' },
                { name: 'State', displayName: 'State', headerCellFilter: 'translate', width: '10%' },
                { name: 'Country.Name', displayName: 'Country', headerCellFilter: 'translate', width: "13%" },
                { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, cellTemplate: "customer/customerAddressBook/customerAddressBookEditButton.tpl.html" }
            ]
        };
    };

    //function for searching Text
    $scope.SearchText = function () {
        if ($scope.TrackSearchText.SearchBy !== 'Select Search Option' && $scope.TrackSearchText.SearchText === '') {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.EnterSearchValue,
                showCloseButton: true
            });
            return;
        }

        AppSpinner.showSpinnerTemplate($scope.LoadingAddressBook, $scope.Template);
        DirectBookingService.GetCustomerAddressBook($scope.TrackSearchText).then(function (response) {
            $scope.AddressDetail = response.data;
            $scope.gridDirectBooking.data = response.data;
            $scope.gridDirectBookingdata = $scope.gridDirectBooking.data;
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    };

    $scope.ChangeAddressType = function (AddressType) {
        $scope.TrackSearchText.AddressType = AddressType;
        $rootScope.getAddressBook();
    };

    $scope.pageChanged = function (TrackSearchText) {
        $rootScope.getAddressBook();
    };

    //function for getting Addressbook
    $rootScope.getAddressBook = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingAddressBook, $scope.Template);
        DirectBookingService.GetCustomerAddressBook($scope.TrackSearchText).then(function (response) {
            $scope.AddressDetail = response.data;
            $scope.gridDirectBookingdata = response.data;
            if ($scope.gridDirectBookingdata !== undefined && $scope.gridDirectBookingdata !== null && $scope.gridDirectBookingdata.length > 0) {
                for (var as = 0; as < $scope.gridDirectBookingdata.length; as++) {
                    $scope.gridDirectBookingdata[as].IsShow = false;
                }
            }
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            AppSpinner.hideSpinnerTemplate();

            var AddressBookMainArray = [];
            var IsFavActiveArray = [];
            var IsFavDeActiveArray = [];


            for (i = 0; i < response.data.length; i++) {
                if (response.data[i].IsFavorites === true) {

                    $scope.star = 'YellowStar';
                    IsFavActiveArray.push(response.data[i]);
                }
                else {
                    $scope.star = 'GreyStar';
                    IsFavDeActiveArray.push(response.data[i]);
                }
            }
            IsFavActiveArray.sort(function (a, b) {
                var x = a.CompanyName.toLowerCase();
                var y = b.CompanyName.toLowerCase();
                if (x < y) { return -1; }
                if (x > y) { return 1; }
                return 0;
            });

            var IsFavActiveEmptyCompany = [];
            var IsFavActiveCompany = [];
            var IsFavDeActiveFinalArray = [];
            for (j = 0; j < IsFavActiveArray.length; j++) {
                if (IsFavActiveArray[j].CompanyName === '') {
                    IsFavActiveEmptyCompany.push(IsFavActiveArray[j]);
                }
                else if (IsFavActiveArray[j].CompanyName !== '') {
                    IsFavActiveCompany.push(IsFavActiveArray[j]);
                }
            }
            IsFavActiveArray = IsFavActiveCompany.concat(IsFavActiveEmptyCompany);


            IsFavDeActiveArray.sort(function (a, b) {
                var x = a.CompanyName.toLowerCase();
                var y = b.CompanyName.toLowerCase();
                if (x < y) { return -1; }
                if (x > y) { return 1; }
                return 0;
            });

            var IsFavDeActiveEmptyCompany = [];
            var IsFavDeActiveCompany = [];

            for (k = 0; k < IsFavDeActiveArray.length; k++) {
                if (IsFavDeActiveArray[k].CompanyName === '') {
                    IsFavDeActiveEmptyCompany.push(IsFavDeActiveArray[k]);
                }
                else if (IsFavDeActiveArray[k].CompanyName !== '') {
                    IsFavDeActiveCompany.push(IsFavDeActiveArray[k]);
                }
            }
            IsFavDeActiveArray = IsFavDeActiveCompany.concat(IsFavDeActiveEmptyCompany);

            $scope.gridDirectBooking.data = IsFavActiveArray.concat(IsFavDeActiveArray);
            $scope.gridDirectBookingdata = IsFavActiveArray.concat(IsFavDeActiveArray);
            $scope.AddressDetail = IsFavActiveArray.concat(IsFavDeActiveArray);
        }, function () {
            AppSpinner.hideSpinnerTemplate();

        });
    };

    //function for Erase All Customers
    $scope.eraseAddressBook = function () {

        var modalOptions = {
            headerText: $scope.Header_Delete,
            bodyText: $scope.Erase_Address
        };

        ModalService.Confirm({}, modalOptions).then(function () {
            DirectBookingService.EraseAllCustomerAddress($scope.TrackSearchText.CustomerId, $scope.TrackSearchText.AddressType).then(function () {
                toaster.pop({
                    type: 'success',
                    title: $scope.Frayte_Success,
                    body: $scope.AddressBookErasedSuccessfully,
                    showCloseButton: true
                });
                //getScreenInitials();
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.ErrorWhileDeletingTheRecords,
                    showCloseButton: true
                });
            });
        });

    };

    //function for Add AddressBook
    $scope.Modalpopup = function (res) {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'customerAddressBookAddEditController',
            templateUrl: 'directBooking/directBookingAddressBook/directBookingAddressBook.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                AddressDetail: function () {
                    if (res !== undefined) {
                        return res;
                    }
                    else {
                        return {
                            AddressbookId: 0,
                            CustomerId: $scope.TrackSearchText.CustomerId,
                            CustomerName: "",
                            FromAddress: $scope.TrackSearchText.AddressType == "FromAddress",
                            ToAddress: $scope.TrackSearchText.AddressType == "ToAddress",
                            ContactFirstName: "",
                            ContactLastName: "",
                            CompanyName: "",
                            Email: "",
                            PhoneNo: "",
                            Address1: "",
                            Area: "",
                            Address2: "",
                            City: "",
                            State: "",
                            Zip: "",
                            Country: null,
                            IsActive: true,
                            TableType: ""
                        };
                    }
                },
                mode: function () {
                    if (res === undefined) {
                        return "Add";
                    }
                    else {
                        return "Edit";
                    }
                }
            }
        });
    };

    $scope.DeleteAddressFromAddressBook = function (res) {
        if (res !== undefined) {
            var modalOptions = {
                headerText: $scope.Header_Delete,
                bodyText: $scope.Deleted_Address
            };

            ModalService.Confirm({}, modalOptions).then(function () {
                DirectBookingService.DeleteCustomerAddress(res.AddressbookId, res.TableType).then(function () {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Frayte_Success,
                        body: $scope.AddressDeletedSuccessfully,
                        showCloseButton: true
                    });
                    $rootScope.getAddressBook();
                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Success,
                        body: $scope.ErrorWhileDeletingTheRecords,
                        showCloseButton: true
                    });
                });
            });
        }
    };

    $scope.Drpdwnfilter = [{
        Id: 1,
        Name: 'All',
        Display: "All"
    },
      {
          Id: 2,
          Name: 'ToAddress',
          Display: "Address To"
      },
      {
          Id: 3,
          Name: 'FromAddress',
          Display: "Address From"
      }
    ];

    $scope.TrackSearchText.AddressType = $scope.Drpdwnfilter[0].Name;

    $scope.setAddressLayOut = function () {

        $scope.viewby = 10;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 100;
        $scope.maxSize = 2;
        $scope.TrackSearchText.TakeRows = $scope.itemsPerPage;
        $scope.TrackSearchText.CurrentPage = $scope.currentPage;
        AppSpinner.showSpinnerTemplate($scope.LoadingAddressBook, $scope.Template);
        $rootScope.getAddressBook();
    };

    $scope.ChangeSearchBy = function (SearchBy) {
        if (SearchBy !== $scope.Selectfilter[0].Name) {
            $scope.Disable = false;
        }
        else {
            $scope.Disable = true;
            $scope.TrackSearchText.SearchText = '';
        }
    };

    // Uplaod address book via excel
    $scope.WhileAddingAddressBookExcel = function ($files, $file, $event) {
        if ($file !== null && $file !== undefined && $file.$error !== undefined) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        AppSpinner.showSpinnerTemplate($scope.WeUploading_YourAddressBook, $scope.Template);
        // Upload the excel file here.
        $scope.uploadExcel = Upload.upload({
            url: config.SERVICE_URL + '/Customer/UploadAddressBookExcel?CustomerId=' + $scope.TrackSearchText.CustomerId,
            file: $file
        }).success(function (response) {
            if (response.Status === true) {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'success',
                    title: $scope.Frayte_Success,
                    body: $scope.Upload_Address_Message,
                    showCloseButton: true
                });
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'customer/customerAddressBook/customerAddressBookError/customerAddressBookError.tpl.html',
                    controller: 'CustomerAddressBookErrorController',
                    windowClass: '',
                    backdrop: true,
                    size: 'md',
                    resolve: {
                        ErrorData: function () {
                            return response.RowErrors;
                        }
                    }
                });
            }
        });
    };

    function init() {

        $scope.Selectfilter = [
            {
                Id: 1,
                Name: 'Select Search Option'
            },
            {
                Id: 2,
                Name: 'Address'
            },
            {
                Id: 3,
                Name: 'City'
            },
            {
                Id: 4,
                Name: 'Country'
            },
            {
                Id: 5,
                Name: 'Postcode'
            },
            {
                Id: 6,
                Name: 'State'
            }
        ];

        $rootScope.ChangeManifest = false;

        $scope.ShowData = 'ShowGridData';
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $rootScope.GetServiceValue = null;
        var userInfo = SessionService.getUser();
        if (userInfo !== undefined) {
            $scope.TrackSearchText.CustomerId = userInfo.EmployeeId;
        }

        AppSpinner.showSpinnerTemplate($scope.LoadingAddressBook, $scope.Template);
        ShipmentService.GetInitials().then(function (response) {
            $scope.Country = TopCountryService.TopCountryList(response.data.Countries);
            $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;
        },

        function () {
            AppSpinner.hideSpinnerTemplate();
        });

        $scope.Disable = true;
        $scope.viewby = 10;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 100;
        $scope.maxSize = 2;
        $scope.TrackSearchText.SearchBy = $scope.Selectfilter[0].Name;
        $scope.TrackSearchText.TakeRows = $scope.itemsPerPage;
        $scope.TrackSearchText.CurrentPage = $scope.currentPage;
        //$rootScope.getAddressBook();
        setGridOptions();
        setModalOptions();
    }

    init();
}]);