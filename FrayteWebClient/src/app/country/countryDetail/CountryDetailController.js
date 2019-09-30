/**
 * Controller
 */
angular.module('ngApp.country').controller('CountryDetailController', function ($scope, $location, $filter, $translate, CountryService, SessionService, $uibModal, $log, toaster, ModalService, $state, uiGridConstants, $stateParams) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['PublicHoliday', 'country', 'Document', 'This', 'DeleteHeader', 'DeleteBody', 'Holiday', 'detail', 'FrayteError', 'FrayteValidation', 'FrayteInformation', 'PleaseCorrectValidationErrors', 'ErrorSavingRecord', 'ErrorDeletingRecord', 'SuccessfullySavedInformation', 'SuccessfullyDelete', 'The', 'document', 'information']).then(function (translations) {
            $scope.headerTextPublicHoliday = translations.PublicHoliday + " " + translations.DeleteHeader;
            $scope.bodyTextPublicHoliday = translations.DeleteBody + " " + translations.This + " " + translations.Holiday;
            $scope.headerTextCountryDocument = translations.country + " " + translations.Document + " " + translations.DeleteHeader;
            $scope.bodyTextCountryDocument = translations.DeleteBody + " " + translations.country + " " + translations.Document + " " + translations.Detail;

            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TitleFrayteError = translations.FrayteError;

            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullyDelete = translations.SuccessfullyDelete + " " + translations.The + " " + translations.country + " " + translations.document + " " + translations.information;
            $scope.TextPublicHolidaySuccessfullyDelete = translations.SuccessfullyDelete + " " + translations.The + " " + translations.PublicHoliday + " " + translations.information;

            $scope.TextErrorDeletingRecord = translations.ErrorDeletingRecord;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
        });
    };

    $scope.GoBack = function () {
        $state.go('admin.country');
    };

    $scope.SaveCountryDetail = function (isValid, countryDetail) {
        if (isValid) {
            var countryId = countryDetail.CountryId;
            CountryService.SaveCountry(countryDetail).then(function (response) {

                $scope.GoBack();

                toaster.pop({
                    type: 'success',
                    title: 'Frayte-Information',
                    body: $scope.TextSuccessfullySavedInformation,
                    showCloseButton: true
                });
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });

            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }
    };

    $scope.AddEditCountryDocuments = function (documentDetail) {
        //Instanciate the new pop-up here.
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'country/countryDocument.tpl.html',
            controller: 'CountryDocumentController',
            windowClass: 'AddCountryDoc-Modal',
            size: 'sm',
            backdrop: 'static',
            resolve: {
                countryDetail: function () {
                    return $scope.countryDetail;
                },
                countryDocument: function () {
                    if (documentDetail === undefined) {
                        return {
                            CountryDocumentId: 0,
                            DocumentName: '',
                            ShipmentType: '',
                            CountryId: $scope.countryDetail.CountryId
                        };
                    }
                    else {
                        return documentDetail;
                    }
                }
            }
        });

        modalInstance.result.then(function (CountryDocuments) {
            //To Dos : Here we need to write the code to add/edit the existing scope            
            $scope.countryDetail.CountryDocuments = CountryDocuments;
        }, function (CountryDocuments) {
            //User cancled the pop-up   
            $scope.countryDetail.CountryDocuments = CountryDocuments;
        });
    };

    $scope.DeleteCountryDocument = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextCountryDocument,
            bodyText: $scope.bodyTextCountryDocument + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            if (row.CountryDocumentId > 0) {
                CountryService.DeleteCountryDocument(row.CountryDocumentId).then(function (response) {
                    if (response.data.Status) {
                        var index = $scope.countryDetail.CountryDocuments.indexOf(row);
                        $scope.countryDetail.CountryDocuments.splice(index, 1);
                        toaster.pop({
                            type: 'success',
                            title: 'Frayte-Information',
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
            }
            else {
                var index = $scope.countryDetail.CountryDocuments.indexOf(row);
                $scope.countryDetail.CountryDocuments.splice(index, 1);
            }
        });
    };

    $scope.filterExpression = function (ShipmentType) {
        return function (countryDocument) {
            return (!countryDocument.DocumentName ||
                      (countryDocument.ShipmentType === ShipmentType));
        };
    };

    $scope.GetGlobalCountryList = function () {
        $scope.GlobalCompanyList = [
            { Name: 'Afghanistan', code: 'AFG' },
            { Name: 'Aland Islands', Code: 'ALA' },
            { Name: 'Albania', Code: 'ALB' },
            { Name: 'Algeria', Code: 'DZA' },
            { Name: 'American Samoa', Code: 'ASM' },
            { Name: 'Andorra', Code: 'AND' },
            { Name: 'Angola', Code: 'AGO' },
            { Name: 'Anguilla', Code: 'AIA' },
            { Name: 'Antigua and Barbuda', Code: 'ATG' },
            { Name: 'Argentina', Code: 'ARG' },
            { Name: 'Armenia', Code: 'ARM' },
            { Name: 'Aruba', Code: 'ABW' },
            { Name: 'Australia', Code: 'AUS' },
            { Name: 'Austria', Code: 'AUT' },
            { Name: 'Azerbaijan', Code: 'AZE' },
            { Name: 'Bahamas', Code: 'BHS' },
            { Name: 'Bahrain', Code: 'BHR' },
            { Name: 'Bangladesh', Code: 'BGD' },
            { Name: 'Barbados', Code: 'BRB' },
            { Name: 'Belarus', Code: 'BLR' },
            { Name: 'Belgium', Code: 'BEL' },
            { Name: 'Belize', Code: 'BLZ' },
            { Name: 'Benin', Code: 'BEN' },
            { Name: 'Bermuda', Code: 'BMU' },
            { Name: 'Bhutan', Code: 'BTN' },
            { Name: 'Bolivia', Code: 'BOL' },
            { Name: 'Bosnia and Herzegovina', Code: 'BIH' },
            { Name: 'Botswana', Code: 'BWA' },
            { Name: 'Brazil', Code: 'BRA' },
            { Name: 'British Virgin Islands', Code: 'VGB' },
            { Name: 'Brunei Darussalam', Code: 'BRN' },
            { Name: 'Bulgaria', Code: 'BGR' },
            { Name: 'Burkina Faso', Code: 'BFA' },
            { Name: 'Burundi', Code: 'BDI' },
            { Name: 'Cambodia', Code: 'KHM' },
            { Name: 'Cameroon', Code: 'CMR' },
            { Name: 'Canada', Code: 'CAN' },
            { Name: 'Cape Verde', Code: 'CPV' },
            { Name: 'Cayman Islands', Code: 'CYM' },
            { Name: 'Central African Republic', Code: 'CAF' },
            { Name: 'Chad', Code: 'TCD' },
            { Name: 'Chile', Code: 'CHL' },
            { Name: 'China', Code: 'CHN' },
            { Name: 'Hong Kong Special Administrative Region of China', Code: 'HKG' },
            { Name: 'Macao Special Administrative Region of China', Code: 'MAC' },
            { Name: 'Colombia', Code: 'COL' },
            { Name: 'Comoros', Code: 'COM' },
            { Name: 'Congo', Code: 'COG' },
            { Name: 'Cook Islands', Code: 'COK' },
            { Name: 'Costa Rica', Code: 'CRI' },
            { Name: 'Côte d Ivoire', Code: 'CIV' },
            { Name: 'Croatia', Code: 'HRV' },
            { Name: 'Cuba', Code: 'CUB' },
            { Name: 'Cyprus', Code: 'CYP' },
            { Name: 'Czech Republic', Code: 'CZE' },
            { Name: 'Democratic Peoples Republic of Korea', Code: 'PRK' },
            { Name: 'Democratic Republic of the Congo', Code: 'COD' },
            { Name: 'Denmark', Code: 'DNK' },
            { Name: 'Djibouti', Code: 'DJI' },
            { Name: 'Dominica', Code: 'DMA' },
            { Name: 'Dominican Republic', Code: 'DOM' },
            { Name: 'Ecuador', Code: 'ECU' },
            { Name: 'Egypt', Code: 'EGY' },
            { Name: 'El Salvador', Code: 'SLV' },
            { Name: 'Equatorial Guinea', Code: 'GNQ' },
            { Name: 'Eritrea', Code: 'ERI' },
            { Name: 'Estonia', Code: 'EST' },
            { Name: 'Ethiopia', Code: 'ETH' },
            { Name: 'Faeroe Islands', Code: 'FRO' },
            { Name: 'Falkland Islands (Malvinas)', Code: 'FLK' },
            { Name: 'Fiji', Code: 'FJI' },
            { Name: 'Finland', Code: 'FIN' },
            { Name: 'France', Code: 'FRA' },
            { Name: 'French Guiana', Code: 'GUF' },
            { Name: 'French Polynesia', Code: 'PYF' },
            { Name: 'Gabon', Code: 'GAB' },
            { Name: 'Gambia', Code: 'GMB' },
            { Name: 'Georgia', Code: 'GEO' },
            { Name: 'Germany', Code: 'DEU' },
            { Name: 'Ghana', Code: 'GHA' },
            { Name: 'Gibraltar', Code: 'GIB' },
            { Name: 'Greece', Code: 'GRC' },
            { Name: 'Greenland', Code: 'GRL' },
            { Name: 'Grenada', Code: 'GRD' },
            { Name: 'Guadeloupe', Code: 'GLP' },
            { Name: 'Guam', Code: 'GUM' },
            { Name: 'Guatemala', Code: 'GTM' },
            { Name: 'Guernsey', Code: 'GGY' },
            { Name: 'Guinea', Code: 'GIN' },
            { Name: 'Guinea-Bissau', Code: 'GNB' },
            { Name: 'Guyana', Code: 'GUY' },
            { Name: 'Haiti', Code: 'HTI' },
            { Name: 'Holy See', Code: 'VAT' },
            { Name: 'Honduras', Code: 'HND' },
            { Name: 'Hungary', Code: 'HUN' },
            { Name: 'Iceland', Code: 'ISL' },
            { Name: 'India', Code: 'IND' },
            { Name: 'Indonesia', Code: 'IDN' },
            { Name: 'Iran, Islamic Republic of', Code: 'IRN' },
            { Name: 'Iraq', Code: 'IRQ' },
            { Name: 'Ireland', Code: 'IRL' },
            { Name: 'Isle of Man', Code: 'IMN' },
            { Name: 'Israel', Code: 'ISR' },
            { Name: 'Italy', Code: 'ITA' },
            { Name: 'Jamaica', Code: 'JAM' },
            { Name: 'Japan', Code: 'JPN' },
            { Name: 'Jersey', Code: 'JEY' },
            { Name: 'Jordan', Code: 'JOR' },
            { Name: 'Kazakhstan', Code: 'KAZ' },
            { Name: 'Kenya', Code: 'KEN' },
            { Name: 'Kiribati', Code: 'KIR' },
            { Name: 'Kuwait', Code: 'KWT' },
            { Name: 'Kyrgyzstan', Code: 'KGZ' },
            { Name: 'Lao Peoples Democratic Republic', Code: 'LAO' },
            { Name: 'Latvia', Code: 'LVA' },
            { Name: 'Lebanon', Code: 'LBN' },
            { Name: 'Lesotho', Code: 'LSO' },
            { Name: 'Liberia', Code: 'LBR' },
            { Name: 'Libyan Arab Jamahiriya', Code: 'LBY' },
            { Name: 'Liechtenstein', Code: 'LIE' },
            { Name: 'Lithuania', Code: 'LTU' },
            { Name: 'Luxembourg', Code: 'LUX' },
            { Name: 'Madagascar', Code: 'MDG' },
            { Name: 'Malawi', Code: 'MWI' },
            { Name: 'Malaysia', Code: 'MYS' },
            { Name: 'Maldives', Code: 'MDV' },
            { Name: 'Mali', Code: 'MLI' },
            { Name: 'Malta', Code: 'MLT' },
            { Name: 'Marshall Islands', Code: 'MHL' },
            { Name: 'Martinique', Code: 'MTQ' },
            { Name: 'Mauritania', Code: 'MRT' },
            { Name: 'Mauritius', Code: 'MUS' },
            { Name: 'Mayotte', Code: 'MYT' },
            { Name: 'Mexico', Code: 'MEX' },
            { Name: 'Micronesia, Federated States of', Code: 'FSM' },
            { Name: 'Moldova', Code: 'MDA' },
            { Name: 'Monaco', Code: 'MCO' },
            { Name: 'Mongolia', Code: 'MNG' },
            { Name: 'Montenegro', Code: 'MNE' },
            { Name: 'Montserrat', Code: 'MSR' },
            { Name: 'Morocco', Code: 'MAR' },
            { Name: 'Mozambique', Code: 'MOZ' },
            { Name: 'Myanmar', Code: 'MMR' },
            { Name: 'Namibia', Code: 'NAM' },
            { Name: 'Nauru', Code: 'NRU' },
            { Name: 'Nepal', Code: 'NPL' },
            { Name: 'Netherlands', Code: 'NLD' },
            { Name: 'Netherlands Antilles', Code: 'ANT' },
            { Name: 'New Caledonia', Code: 'NCL' },
            { Name: 'New Zealand', Code: 'NZL' },
            { Name: 'Nicaragua', Code: 'NIC' },
            { Name: 'Niger', Code: 'NER' },
            { Name: 'Nigeria', Code: 'NGA' },
            { Name: 'Niue', Code: 'NIU' },
            { Name: 'Norfolk Island', Code: 'NFK' },
            { Name: 'Northern Mariana Islands', Code: 'MNP' },
            { Name: 'Norway', Code: 'NOR' },
            { Name: 'Occupied Palestinian Territory', Code: 'PSE' },
            { Name: 'Oman', Code: 'OMN' },
            { Name: 'Pakistan', Code: 'PAK' },
            { Name: 'Palau', Code: 'PLW' },
            { Name: 'Panama', Code: 'PAN' },
            { Name: 'Papua New Guinea', Code: 'PNG' },
            { Name: 'Paraguay', Code: 'PRY' },
            { Name: 'Peru', Code: 'PER' },
            { Name: 'Philippines', Code: 'PHL' },
            { Name: 'Pitcairn', Code: 'PCN' },
            { Name: 'Poland', Code: 'POL' },
            { Name: 'Portugal', Code: 'PRT' },
            { Name: 'Puerto Rico', Code: 'PRI' },
            { Name: 'Qatar', Code: 'QAT' },
            { Name: 'Republic of Korea', Code: 'KOR' },
            { Name: 'R_union', Code: 'REU' },
            { Name: 'Romania', Code: 'ROU' },
            { Name: 'Russian Federation', Code: 'RUS' },
            { Name: 'Rwanda', Code: 'RWA' },
            { Name: 'Saint-Barthélemy', Code: 'BLM' },
            { Name: 'Saint Helena', Code: 'SHN' },
            { Name: 'Saint Kitts and Nevis', Code: 'KNA' },
            { Name: 'Saint Lucia', Code: 'LCA' },
            { Name: 'Saint-Martin (French part)', Code: 'MAF' },
            { Name: 'Saint Pierre and Miquelon', Code: 'SPM' },
            { Name: 'Saint Vincent and the Grenadines', Code: 'VCT' },
            { Name: 'Samoa', Code: 'WSM' },
            { Name: 'San Marino', Code: 'SMR' },
            { Name: 'Sao Tome and Principe', Code: 'STP' },
            { Name: 'Saudi Arabia', Code: 'SAU' },
            { Name: 'Senegal', Code: 'SEN' },
            { Name: 'Serbia', Code: 'SRB' },
            { Name: 'Seychelles', Code: 'SYC' },
            { Name: 'Sierra Leone', Code: 'SLE' },
            { Name: 'Singapore', Code: 'SGP' },
            { Name: 'Slovakia', Code: 'SVK' },
            { Name: 'Slovenia', Code: 'SVN' },
            { Name: 'Solomon Islands', Code: 'SLB' },
            { Name: 'Somalia', Code: 'SOM' },
            { Name: 'South Africa', Code: 'ZAF' },
            { Name: 'Spain', Code: 'ESP' },
            { Name: 'Sri Lanka', Code: 'LKA' },
            { Name: 'Sudan', Code: 'SDN' },
            { Name: 'Suriname', Code: 'SUR' },
            { Name: 'Svalbard and Jan Mayen Islands', Code: 'SJM' },
            { Name: 'Swaziland', Code: 'SWZ' },
            { Name: 'Sweden', Code: 'SWE' },
            { Name: 'Switzerland', Code: 'CHE' },
            { Name: 'Syrian Arab Republic', Code: 'SYR' },
            { Name: 'Tajikistan', Code: 'TJK' },
            { Name: 'Thailand', Code: 'THA' },
            { Name: 'The former Yugoslav Republic of Macedonia', Code: 'MKD' },
            { Name: 'Timor-Leste', Code: 'TLS' },
            { Name: 'Togo', Code: 'TGO' },
            { Name: 'Tokelau', Code: 'TKL' },
            { Name: 'Tonga', Code: 'TON' },
            { Name: 'Trinidad and Tobago', Code: 'TTO' },
            { Name: 'Tunisia', Code: 'TUN' },
            { Name: 'Turkey', Code: 'TUR' },
            { Name: 'Turkmenistan', Code: 'TKM' },
            { Name: 'Turks and Caicos Islands', Code: 'TCA' },
            { Name: 'Tuvalu', Code: 'TUV' },
            { Name: 'Uganda', Code: 'UGA' },
            { Name: 'Ukraine', Code: 'UKR' },
            { Name: 'United Arab Emirates', Code: 'ARE' },
            { Name: 'United Kingdom of Great Britain and Northern Ireland', Code: 'GBR' },
            { Name: 'United Republic of Tanzania', Code: 'TZA' },
            { Name: 'United States of America', Code: 'USA' },
            { Name: 'United States Virgin Islands', Code: 'VIR' },
            { Name: 'Uruguay', Code: 'URY' },
            { Name: 'Uzbekistan', Code: 'UZB' },
            { Name: 'Vanuatu', Code: 'VUT' },
            { Name: 'Venezuela (Bolivarian Republic of)', Code: 'VEN' },
            { Name: 'Viet Nam', Code: 'VNM' },
            { Name: 'Wallis and Futuna Islands', Code: 'WLF' },
            { Name: 'Western Sahara', Code: 'ESH' },
            { Name: 'Yemen', Code: 'YEM' },
            { Name: 'Zambia', Code: 'ZMB' },
            { Name: 'Zimbabwe', Code: 'ZWE' }
        ];
    };

    //Start : Public Holiday Panel
    $scope.RearrangeSerialNumbers = function (collectionObject, objectType) {
        if (collectionObject.length > 0) {
            for (var i = 0; i < collectionObject.length; i++) {
                collectionObject[i].SN = i + 1;
            }
        }
        if (objectType === 'PH') {
            $scope.countryDetail.CountryPublicHolidays = collectionObject;
        }
    };

    //PublicHolidayPanel Show or Hide function.    
    $scope.ShowHidePublicHolidayPanel = function () {
        $scope.ShowPublicHolidayPanel = !$scope.ShowPublicHolidayPanel;
    };

    $scope.AddEditCountryHoliday = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'country/countryHoliday/countryHolidayAddEdit.tpl.html',
            controller: 'CountryHolidayAddEditController',
            windowClass: '',
            size: 'md',
            backdrop: 'static',
            resolve: {
                mode: function () {
                    if (row === undefined) {
                        return 'Add';
                    }
                    else {
                        return 'Modify';
                    }
                },
                publicHolidays: function () {
                    return $scope.countryDetail.CountryPublicHolidays;
                },
                publicHoliday: function () {
                    if (row === undefined) {
                        return {
                            SN: 0,
                            CountryPublicHolidayId: 0,
                            PublicHolidayDate: null,
                            Description: ''
                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });

        modalInstance.result.then(function (publicHolidays) {
            $scope.countryDetail.CountryPublicHolidays = publicHolidays;
        });

    };

    $scope.DeleteCountryHolidays = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextPublicHoliday,
            bodyText: $scope.bodyTextPublicHoliday + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
          
            // Delete Public Holiday From Database 
            CountryService.DeleteCountryHoliday(row.entity.CountryPublicHolidayId).then(function (response) {                
                if (response.data.Status) {

                    //Remove the row from shipperDetail.PublicHoliday collerction(array)
                    var index = $scope.countryDetail.CountryPublicHolidays.indexOf(row.entity);
                    $scope.countryDetail.CountryPublicHolidays.splice(index, 1);
                    $scope.RearrangeSerialNumbers($scope.countryDetail.CountryPublicHolidays, 'PH');

                    toaster.pop({
                        type: 'success',
                        title: 'Frayte-Information',
                        body: $scope.TextPublicHolidaySuccessfullyDelete,
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

    $scope.SetGridOptions = function () {
        $scope.gridOptionsPublicHoliday = {
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
            enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
            columnDefs: [
              { name: 'SN', headerCellFilter: 'translate', enableFiltering: false, visible: false },
              { name: 'PublicHolidayDate', displayName: 'HolidayDate', headerCellFilter: 'translate', cellFilter: 'dateFilter:this' },
              { name: 'Description', headerCellFilter: 'translate' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "country/countryHoliday/countryHolidayEditButton.tpl.html", width: 65 }
            ]
        };
    };
    //End : Public Holiday Panel

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.SetGridOptions();

        $scope.countryId = $stateParams.CountryId;

        CountryService.GetCountryDetail($scope.countryId).then(function (response) {
            $scope.countryDetail = response.data;
            $scope.RearrangeSerialNumbers($scope.countryDetail.CountryPublicHolidays, 'PH');
            $scope.gridOptionsPublicHoliday.data = $scope.countryDetail.CountryPublicHolidays;
        });

        //hide Public Holiday Panel
        $scope.ShowPublicHolidayPanel = false;
    }

    init();
});