/** 
 * Controller
 */
angular.module('ngApp.country').controller('CountryController', function ($scope, $state, $location, $filter, $translate, CountryService, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['Country', 'DeleteHeader', 'This', 'The', 'country', 'information', 'records', 'DeleteBody', 'Detail', 'FrayteError', 'FrayteInformation', 'ErrorDeletingRecord', 'ErrorGetting', 'SuccessfullyDelete']).then(function (translations) {
            $scope.headerTextCountry = translations.Country + " " + translations.DeleteHeader;
            $scope.bodyTextCountry = translations.DeleteBody + " " + translations.This + " " + translations.Country + " " + translations.Detail;

            $scope.TitleFrayteError = translations.FrayteError;            
            $scope.TitleFrayteInformation = translations.FrayteInformation;

            $scope.TextSuccessfullyDelete = translations.SuccessfullyDelete + " " + translations.The + " " + translations.country + " " + translations.information;
            $scope.TextErrorDeletingRecord = translations.ErrorDeletingRecord;
            $scope.TextErrorGettingCountryRecord = translations.ErrorGetting + " " + translations.country + " " + translations.records;                        
        });
    };

    $scope.countryList = [];
    $scope.AddEditCountry = function (row) {
        $state.go('admin.country-detail', { "CountryId": row.entity.CountryId });
    };

    $scope.DeleteCountry = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextCountry,
            bodyText: $scope.bodyTextCountry + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            CountryService.DeleteCountry(row.entity.CountryId).then(function (response) {
                if (response.data.Status) {
                    var index = $scope.gridOptions.data.indexOf(row.entity);
                    $scope.gridOptions.data.splice(index, 1);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.TextSuccessfullyDelete,
                        showCloseButton: true
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteError,
                        body: response.data.Errors[0],
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorDeletingRecord,
                    showCloseButton: true
                });
            });
        });
    };

    var statusTemplate = '<div class="ui-grid-cell-contents">{{COL_FIELD == true ? "Yes" : "No"}}</div>';

    $scope.SetGridOptions = function () {
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
            enableGridMenu: false,
            enableColumnMenus: false,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            //data: $scope.countries,
            columnDefs: [
              { name: 'CountryId', visible: false },
              { name: 'Name', displayName: 'CountryName', headerCellFilter: 'translate' },
              { name: 'Code', headerCellFilter: 'translate' },
              { name: 'Air', cellTemplate: statusTemplate, headerCellFilter: 'translate' },
              { name: 'Courier', cellTemplate: statusTemplate, headerCellFilter: 'translate' },
              { name: 'Expryes', cellTemplate: statusTemplate, headerCellFilter: 'translate' },
              { name: 'Sea', cellTemplate: statusTemplate, headerCellFilter: 'translate' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "country/countryEditButton.tpl.html", width: 65 }
            ]
        };
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        CountryService.GetCountryList().then(function (response) {

            $scope.countries = response.data;
            $scope.gridOptions.data = response.data;
            //For full List/Data in UI-GRID
            $scope.gridOptions.excessRows = $scope.gridOptions.data.length;

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingCountryRecord,
                showCloseButton: true
            });
        });
    }

    init();

});