angular.module('ngApp.AddressBook').controller('CustomerAddressBookController', ['$scope', 'AppSpinner', 'DirectBookingService', '$uibModal', 'SessionService', 'ModalService', 'TopCountryService', 'toaster', 'ShipmentService', '$rootScope', '$translate', 'uiGridConstants', function ($scope, AppSpinner,DirectBookingService, $uibModal, SessionService, ModalService, TopCountryService, toaster, ShipmentService, $rootScope, $translate, uiGridConstants) {
    

    $scope.TrackSearchText = {
        CustomerId: null,
        SearchText: '',
        SearchBy: '',
        AddressType: 'All',
        TakeRows: null,
        CurrentPage: null

    };

    var setModalOptions = function () {
        $translate(['Frayte_Error', 'FrayteWarning', 'FrayteSuccess', 'Erase_AllAddress', 'Delete_Confirmation', 'Delete_Address', 'Successfull', 'FrayteError_Validation', 'AddressBook_Erased_Successfully', 'Error_While_Deleting_The_Records', 'Address_Deleted_Successfully']).then(function (translations) {

            $scope.Header_Delete = translations.Delete_Confirmation;
            $scope.Erase_Address = translations.Erase_AllAddress;
            $scope.Deleted_Address = translations.Delete_Address;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.FrayteWarning = translations.FrayteWarning;
            
            $scope.FrayteError = translations.Frayte_Error;
            $scope.AddressBookErasedSuccessfully = translations.AddressBook_Erased_Successfully;
            $scope.ErrorWhileDeletingTheRecords = translations.Error_While_Deleting_The_Records;
            $scope.AddressDeletedSuccessfully = translations.Address_Deleted_Successfully;
           

        });
    };
    var clientNameTemplate = '<div class="ui-grid-cell-contents">{{grid.appScope.GetClientName(row)}}</div>';

    $scope.SetIsFavourite = function (row) {
        //row.entity.IsFavorites = true;

        DirectBookingService.SaveAddressBook(row).then(function (response) {
            $scope.IsFavouriteData = response.data;
            if (response.status === 200) {
                $rootScope.getAddressBook();
                //DirectBookingService.GetCustomerAddressBook($scope.TrackSearchText).then(function (response) {
                //    $scope.gridDirectBooking.data = response.data;


                //    $scope.AddressBook = response.data;




                //}, function () {

                //});
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
            enableGridMenu: true,
            enableRowSelection: true,
            enableRowHeaderSelection: false,
            enableColumnMenus: false,
            modifierKeysToMultiSelect: false,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            rowTemplate: "<div ng-dblclick=\"grid.appScope.showInfo(row)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>",
            columnDefs: [
                { name: 'IsFavourite', displayName: '', headerCellFilter: 'translate', cellTemplate: 'directBooking/directbookingAddressBookStar.tpl.html', enableFiltering: false, width: "5%" },
                             { name: 'CompanyName', displayName: 'Company', headerCellFilter: 'translate', width: "15%" },
                         { name: 'CustomerName', displayName: 'Client_Name', headerCellFilter: 'translate', width: "15%" },
                           { name: 'Address', displayName: 'Address_1', headerCellFilter: 'translate', width: "20%" },
                       { name: 'Address2', displayName: 'Address_2', headerCellFilter: 'translate', width: "20%" },
                       { name: 'Country.Name', displayName: 'Country', headerCellFilter: 'translate', width: "13%" },
                       { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, cellTemplate: "customer/customerAddressBook/customerAddressBookEditButton.tpl.html" }

            ]
        };
    };

    //function for searching Text
    $scope.SearchText = function () {
        AppSpinner.showSpinnerTemplate("Loading Address Book", $scope.Template);
        DirectBookingService.GetCustomerAddressBook($scope.TrackSearchText).then(function (response) {
            $scope.AddressDetail = response.data;
            $scope.gridDirectBooking.data = response.data;
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

        DirectBookingService.GetCustomerAddressBook($scope.TrackSearchText).then(function (response) {
            $scope.AddressDetail = response.data;
            $scope.gridDirectBooking.data = response.data;
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
            //templateUrl: 'customer/customerAddressBook/customerAddressBookAddEdit/customerAddressBookAddEdit.tpl.html',
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
                //headerText: "Delete Confirmation",
                //bodyText: "Are your sure want to delete the addresses?"
            };

            ModalService.Confirm({}, modalOptions).then(function () {

                DirectBookingService.DeleteCustomerAddress(res.AddressbookId, res.TableType).then(function () {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Frayte_Success,
                        body:  $scope.AddressDeletedSuccessfully,
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
        Display :"All"
    },
      {
          Id: 2,
          Name: 'ToAddress',
          Display: "To Address"
      },
      {
          Id: 3,
          Name: 'FromAddress',
          Display: "From Address"
      }
    ];
    $scope.TrackSearchText.AddressType = $scope.Drpdwnfilter[0].Name;

    $scope.Selectfilter = [{
        Id: 1,
        Name: 'Country'
    },
    {
        Id: 2,
        Name: 'State'
    },
    {
        Id: 3,
        Name: 'City'
    },
    {
        Id: 4,
        Name: 'Address'
    },
    {
        Id: 5,
        Name: 'PostCode'
    }
    ];
    //$scope.TrackSearchText.SearchBy = $scope.Selectfilter[0].Name;

    // Set Country Phone Code
    //setAddressBookinfo = function (Country) {
    //    if (Country !== undefined && Country !== null) {
    //        for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
    //            if ($scope.CountryPhoneCodes[i].CountryCode === Country[i].Code) {
    //                $scope.AddressDetail[i].Phone = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";

    //            }
    //        }
    //    }
    //};

    $scope.setAddressLayOut = function () {

        $scope.viewby = 9;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 100;
        $scope.maxSize = 2;
        $scope.TrackSearchText.TakeRows = $scope.itemsPerPage;
        $scope.TrackSearchText.CurrentPage = $scope.currentPage;
        AppSpinner.showSpinnerTemplate("Loading Address Book", $scope.Template);
        $rootScope.getAddressBook();
    };
    function init() {

        $scope.ShowData = 'ShowGridData';
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $rootScope.GetServiceValue = null;
        var userInfo = SessionService.getUser();
        if (userInfo !== undefined) {
            $scope.TrackSearchText.CustomerId = userInfo.EmployeeId;

        }
        //DirectBookingService.GetInitials($scope.TrackSearchText.CustomerId).then(function (response) {
        //    $scope.Country = response.data.Countries;
        //    setAddressBookinfo($scope.Country);
        //});

        AppSpinner.showSpinnerTemplate("Loading Address Book", $scope.Template);
        ShipmentService.GetInitials().then(function (response) {
            $scope.Country = TopCountryService.TopCountryList(response.data.Countries);
            $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;
            //if ($scope.CountryPhoneCodes !== undefined) {
            //    setAddressBookinfo($scope.Country);
            //}
        },
        function () {
            AppSpinner.hideSpinnerTemplate();
        });
        
        //setAddressBookinfo($scope.Country);


        $scope.viewby = 9;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 100;
        $scope.maxSize = 2;
        $scope.TrackSearchText.TakeRows = $scope.itemsPerPage;
        $scope.TrackSearchText.CurrentPage = $scope.currentPage;
        $rootScope.getAddressBook();
        setGridOptions();
        setModalOptions();
    }
    init();
}]);