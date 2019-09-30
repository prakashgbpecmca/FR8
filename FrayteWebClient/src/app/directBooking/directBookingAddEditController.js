angular.module('ngApp.directBooking').controller('DirectBookingAddEditController', function ($scope, $uibModalInstance, toCountryId, moduleType, ModalService, Countries, addressType, customerId, $filter, CountryPhoneCodes, $state, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService, uiGridConstants) {
    var clientNameTemplate = '<div class="ui-grid-cell-contents">{{grid.appScope.GetClientName(row)}}</div>';
    
    $scope.GetClientName = function (row) {
        if (row === undefined) {
            return '';
        }
        else {
            var str = row.entity.FirstName;
            var str1 = row.entity.LastName;

            return str + ' ' + str1;
        }
    };

    var setMultilingualOptions = function () {
        $translate(['FrayteSuccess', 'AddressBook_Erased_Successfully', 'Address_Deleted_Successfully', 'Error_While_Deleting_The_Records', 'FrayteError', 'ErrorDeletingRecord', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'FrayteWarning_Validation', 'PleaseCorrectValidationErrors']).then(function (translations) {
            $scope.Success = translations.FrayteSuccess;
            $scope.Frayte_Error = translations.FrayteError;
            $scope.ErrorDeleting_Record = translations.ErrorDeletingRecord;
            $scope.AddressDeleted_Successfully = translations.Address_Deleted_Successfully;
            $scope.AddressBookErased_Successfully = translations.AddressBook_Erased_Successfully;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.FrayteErrorValidation = translations.FrayteError_Validation;
            $scope.ErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;

        });
    };

    $scope.SetIsFavourite = function (row) {
        //row.entity.IsFavorites = true;

        DirectBookingService.SaveAddressBook(row).then(function (response) {
            $scope.IsFavouriteData = response.data;
            if(response.status === 200)
            {
                getScreenInitials();
                //DirectBookingService.GetCustomerAddressBook($scope.TrackSearchText).then(function (response) {
                //    $scope.gridDirectBooking.data = response.data;

                   
                //    $scope.AddressBook = response.data;

                 


                //}, function () {

                //});
            }
        });
    };

    $scope.addEditAddressBook = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingAddressBook/directBookingAddressBook.tpl.html',
            controller: 'DirectBookingAddressBookController',
            size: 'lg',
            //windowClass: 'CustomerAddress-Edit',
            backdrop: 'static',
            resolve: {
                Countries: function () {
                    return $scope.AddressBookCountries;
                },
                PhoneCodes: function () {
                    return $scope.AddressBookPhoneCodes;
                },
                AddressDetail: function () {
                    if (row !== undefined) {
                        return row.entity;
                    }
                    else {
                        return {
                            AddressbookId: 0,
                            CustomerId: $scope.customerId,
                            CustomerName: "",
                            FromAddress: $scope.AddressBookAddressType.AddressType === "FromAddress",
                            ToAddress: $scope.AddressBookAddressType.AddressType === "ToAddress",
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
                    if (row === undefined) {
                        return "Add";
                    }
                    else {
                        return "Edit";
                    }
                }
            }
        });

        modalInstance.result.then(function () {
            getScreenInitials();
        }, function () {

        });
    };
    $scope.selectAddressBook = function (row) {
        $uibModalInstance.close($scope.gridApi.selection.getSelectedGridRows());
    };

    $scope.showInfo = function () {
        $uibModalInstance.close($scope.gridApi.selection.getSelectedGridRows());
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
                             { name: 'IsFavourite', displayName: '', headerCellFilter: 'translate', cellTemplate: 'directBooking/directbookingAddressBookStar.tpl.html', width: "5%", enableFiltering: false },
                             { name: 'CompanyName', displayName: 'Company', headerCellFilter: 'translate', width: "16%" },
                             { name: 'CustomerName', displayName: 'ContactMenu', headerCellFilter: 'translate', cellTemplate: clientNameTemplate, width: "15%" },
                             { name: 'Address', displayName: 'Address_1', headerCellFilter: 'translate', width: "20%" },
                             { name: 'Address2', displayName: 'Address_2', headerCellFilter: 'translate', width: "20%" },
                             { name: 'Country.Name', displayName: 'Country', headerCellFilter: 'translate', width: "12%" },
                             { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, cellTemplate: "directBooking/directShipmentAddressBookEditButton.tpl.html" }
                       ]
        };
    };
    $scope.eraseAddressBook = function () {

        var modalOptions = {
            headerText: "Delete Confirmation",
            bodyText: "Are your sure want to erase all addresses?"
        };

        ModalService.Confirm({}, modalOptions).then(function () {
            DirectBookingService.EraseAllCustomerAddress($scope.customerId, $scope.AddressType).then(function () {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.AddressBookErased_Successfully,
                    showCloseButton: true
                });
                getScreenInitials();
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: $scope.ErrorDeleting_Record,
                    showCloseButton: true
                });
            });
        });

    };
    $scope.DeleteAddressFromAddressBook = function (row) {
        if (row !== undefined) {
            var modalOptions = {
                headerText: "Delete Confirmation",
                bodyText: "Are your sure want to delete the addresses?"
            };

            ModalService.Confirm({}, modalOptions).then(function () {

                DirectBookingService.DeleteCustomerAddress(row.entity.AddressbookId, row.entity.TableType).then(function () {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.AddressDeleted_Successfully,
                        showCloseButton: true
                    });
                    getScreenInitials();
                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.ErrorDeleting_Record,
                        showCloseButton: true
                    });
                });

            });

        }

    };




    var getScreenInitials = function () {
        DirectBookingService.GetCustomerAddressBook($scope.TrackSearchText).then(function (response) {
            //$scope.gridDirectBooking.data = response.data;
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            $scope.AddressBook = response.data;
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
            //$scope.gridApi.core.notifyDataChange($scope.gridApi.grid, uiGridConstants.dataChange.COLUMN);
        }, function () {

        });
    };
    $scope.submit = function () {
        $uibModalInstance.close($scope.gridApi.selection.getSelectedGridRows());
    };

    $scope.AddressBookByAddressType = function () {
        if ($scope.AddressBookAddressType !== null) {
            $scope.TrackSearchText.AddressType = $scope.AddressBookAddressType.AddressType;

            getScreenInitials();
        }
    };

    $scope.pageChanged = function (TrackSearchText) {
        getScreenInitials();
    };

    function init() {
        $scope.maxSize = 2;
        $scope.AddressTypes = [
            {
                Id: 1,
                AddressType: "All",
                AddressTypeDisplay: "All"
            },
           {
               Id: 2,
               AddressType: "FromAddress",
               AddressTypeDisplay: "From Address"
           },
           {
               Id: 3,
               AddressType: "ToAddress",
               AddressTypeDisplay: "To Address"
           }
        ];
        $scope.AddressBookAddressType = null;
        $scope.AddressBookCountries = Countries;
        $scope.AddressBookPhoneCodes = CountryPhoneCodes;
        $scope.ModuleType = moduleType;
        $scope.AddressType = addressType;
        $scope.toCountryId = toCountryId;
        $scope.customerId = customerId;
        $scope.TrackSearchText = {
            CustomerId: customerId,
            AddressSearch: '',
            SearchText: '',
            SearchBy: '',
            AddressType: '',
            TakeRows: 50,
            CurrentPage: 1,
            ModuleType: $scope.ModuleType,
            CountryId: $scope.toCountryId
        };
        var found = $filter('filter')($scope.AddressTypes, { AddressType: $scope.AddressType });
        if (found.length) {
            $scope.AddressBookAddressType = found[0];
            $scope.TrackSearchText.AddressType = found[0].AddressType;
            $scope.TrackSearchText.AddressSearch = found[0].AddressType;
        }

        //$scope.TrackSearchText.customerId = customerId;
        setGridOptions();
        $scope.gridDirectBooking.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };
        getScreenInitials();
        setMultilingualOptions();

    }

    init();

});