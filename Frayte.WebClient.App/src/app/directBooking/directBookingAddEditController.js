angular.module('ngApp.directBooking').controller('DirectBookingAddEditController', function ($scope, $window, $document, $uibModalInstance, toCountryId, moduleType, ModalService, Countries, addressType, customerId, $filter, CountryPhoneCodes, $state, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService, uiGridConstants) {

    var clientNameTemplate = '<div>{{grid.appScope.GetClientName(row)}}</div>';

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

    $scope.selectAdressRecord = function (gridBooking) {
        if ($scope.gridDirectBookingdata !== undefined && $scope.gridDirectBookingdata !== null && $scope.gridDirectBookingdata.length > 0) {
            for (var i = 0; i < $scope.gridDirectBookingdata.length; i++) {
                $scope.gridDirectBookingdata[i].IsSelected = false;

            }
        }
        if (gridBooking !== null && gridBooking !== undefined) {
            gridBooking.IsSelected = true;
            if (gridBooking.Address.length > 35) {
                gridBooking.Address = gridBooking.Address.substring(0, 34);
            }
            if (gridBooking.Address2.length > 35) {
                gridBooking.Address2 = gridBooking.Address2.substring(0, 34);
            }

            $scope.GridData = gridBooking;
        }
    };

    var setMultilingualOptions = function () {
        $translate(['FrayteSuccess', 'AddressBook_Erased_Successfully', 'Address_Deleted_Successfully', 'Error_While_Deleting_The_Records', 'FrayteError', 'ErrorDeletingRecord', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'FrayteWarning_Validation', 'PleaseCorrectValidationErrors', 'PleaseSelectAddressFirst',
        'Delete_Confirmation', 'SureToEraseAllAddress', 'SureToDeleteAddress']).then(function (translations) {
            $scope.Success = translations.FrayteSuccess;
            $scope.Frayte_Error = translations.FrayteError;
            $scope.ErrorDeleting_Record = translations.ErrorDeletingRecord;
            $scope.AddressDeleted_Successfully = translations.Address_Deleted_Successfully;
            $scope.AddressBook_Erased_Successfully = translations.AddressBook_Erased_Successfully;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.FrayteErrorValidation = translations.FrayteError_Validation;
            $scope.ErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.SelectAddress = translations.PleaseSelectAddressFirst;
            $scope.Delete_Confirmation = translations.Delete_Confirmation;
            $scope.SureToEraseAllAddress = translations.SureToEraseAllAddress;
            $scope.SureToDeleteAddress = translations.SureToDeleteAddress;


        });
    };

    $scope.SetIsFavourite = function (row) {
        DirectBookingService.MarkAddressAsFavourite(row.AddressbookId, row.IsFavorites).then(function (response) {
            //$scope.IsFavouriteData = response.data;
            if (response.status === 200) {
                getScreenInitials();
            }
        });
    };

    $scope.addEditAddressBook = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingAddressBook/directBookingAddressBook.tpl.html',
            controller: 'DirectBookingAddressBookController',
            size: 'lg',
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
                        return row;
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
        if ($scope.setDefaultAddress) {
            row.IsDefault = true;
        }

        $uibModalInstance.close(row);
    };

    $scope.showInfo = function () {
        $uibModalInstance.close($scope.gridApi.selection.getSelectedGridRows());
    };

    $scope.MousesOver = function () {
        $scope.abc = true;
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
            rowTemplate: "<div id=\"{{$index}}\" ng-dblclick=\"grid.appScope.showInfo(row)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell grid-popup\" cellClass=\"width100 pointer flex\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\"  popover-placement=\"\" uib-popover=\"Double click to select the service.\" popover-append-to-body = true popover-trigger=\"\'mouseenter\'\"ui-grid-cell ></div>",
            columnDefs: [
                             { name: 'IsFavourite', displayName: 'Favourite', headerCellFilter: 'translate', cellTemplate: 'directBooking/directbookingAddressBookStar.tpl.html', width: "10%", enableFiltering: false },
                             { name: 'CompanyName', displayName: 'Company', headerCellFilter: 'translate', width: "16%" },
                             { name: 'CustomerName', displayName: 'ContactMenu', headerCellFilter: 'translate', cellTemplate: clientNameTemplate, width: "12%" },
                             { name: 'Address', displayName: 'Address_1', headerCellFilter: 'translate', width: "20%" },
                             { name: 'Address2', displayName: 'Address_2', headerCellFilter: 'translate', width: "18%" },
                             { name: 'Country.Name', displayName: 'Country', headerCellFilter: 'translate', width: "12%" },
                             { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, cellTemplate: "directBooking/directShipmentAddressBookEditButton.tpl.html" }
            ]
        };
    };

    $scope.eraseAddressBook = function () {

        var modalOptions = {
            headerText: $scope.Delete_Confirmation,
            bodyText: $scope.SureToEraseAllAddress
        };

        ModalService.Confirm({}, modalOptions).then(function () {
            DirectBookingService.EraseAllCustomerAddress($scope.customerId, $scope.AddressType).then(function () {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.AddressBook_Erased_Successfully,
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

    var removeDefaultAddressFromGrid = function () {
        for (i = 0; i < $scope.gridDirectBookingdata.length; i++) {
            if ($scope.gridDirectBookingdata[i].IsDefault) {
                $scope.gridDirectBookingdata[i].IsDefault = false;
                break;
            }
        }
    };

    $scope.setAsDefaultAddress = function (row) {

      
        var AddressType = "";
        if (row.FromAddress) {
            AddressType = "FromAddress";
        }
        if (row.ToAddress) {
            AddressType = "ToAddress";
        }

        DirectBookingService.CustomerDefaultAddress(row.AddressbookId, row.Country.CountryId, $scope.TrackSearchText.CustomerId, AddressType).then(function (response) {
            if (response.data.Status) {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: "Successfully added as default address",
                    showCloseButton: true
                });
                removeDefaultAddressFromGrid();
                row.IsDefault = true;
                $scope.setDefaultAddress = true; 

            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: "Could not set the default address.",
                    showCloseButton: true
                });
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: "Could not set the default address.",
                showCloseButton: true
            });
        });

    };

    $scope.DeleteAddressFromAddressBook = function (row) {
        if (row !== undefined) {
            var modalOptions = {
                headerText: $scope.Delete_Confirmation,
                bodyText: $scope.SureToDeleteAddress
            };

            ModalService.Confirm({}, modalOptions).then(function () {

                DirectBookingService.DeleteCustomerAddress(row.AddressbookId, row.TableType).then(function () {
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
            $scope.gridDirectBookingdata = IsFavActiveArray.concat(IsFavDeActiveArray);

            for (i = 0; i < $scope.gridDirectBookingdata.length; i++) {
                $scope.gridDirectBookingdata.IsSelected = false;
            }
        }, function () {

        });
    };

    $scope.submit = function () {
        //$uibModalInstance.close($scope.getSelectedGridRows());
        if ($scope.GridData === null || $scope.GridData === undefined) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.SelectAddress,
                showCloseButton: true
            });
        }
        else {
            $uibModalInstance.close($scope.GridData);
        }
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

    $scope.Cancel = function () {
        $uibModalInstance.close();
        getScreenInitials();
    };

    $scope.Dismiss1 = function () {
        $uibModalInstance.close();
        getScreenInitials();
    };

    function init() {
        $scope.setDefaultAddress = false;
        $scope.maxSize = 2;
        $scope.AddressTypes = [
            {
                Id: 1,
                AddressType: "All",
                AddressTypeDisplay: "All"
            },
            {
                Id: 2,
                AddressType: "ToAddress",
                AddressTypeDisplay: "Address To"
            },
            {
                Id: 3,
                AddressType: "FromAddress",
                AddressTypeDisplay: "Address From"
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
            TakeRows: 10,
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

        setGridOptions();
        $scope.gridDirectBooking.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };
        getScreenInitials();
        setMultilingualOptions();


        $scope.viewby = 10;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 100;
        $scope.maxSize = 2;
        $scope.TrackSearchText.TakeRows = $scope.itemsPerPage;
        $scope.TrackSearchText.CurrentPage = $scope.currentPage;

        var elen = document.getElementsByClassName("modal-dialog");
    }

    init();

});