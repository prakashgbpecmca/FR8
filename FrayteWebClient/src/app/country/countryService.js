/**
 * Service
 */
angular.module('ngApp.country').factory('CountryService', function ($http, config, SessionService) {

    var GetCountryList = function () {
        return $http.get(config.SERVICE_URL + '/Country/GetCountyList');
    };

    var GetCountryCodeList = function () {
        return $http.get(config.SERVICE_URL + '/Country/GetCountryCodeList');
    };


    var GetCountyDocumentList = function (countryId) {
        return $http.get(config.SERVICE_URL + '/Country/GetCountyDocumentList',
            {
                params: {
                    countryId: countryId
                }
            });
    };

    var DeleteCountryHoliday = function (countryHolydayId) {
        return $http.get(config.SERVICE_URL + '/Country/DeleteCountryHoliday', {
            params: {
                countryHolydayId: countryHolydayId
            }
        });
    };

    var GetAllCountryList = function () {
        return $http.get(config.SERVICE_URL + '/Country/GetAllCountyList');
    };

    var SaveCountry = function (countryDetail) {
        return $http.post(config.SERVICE_URL + '/Country/SaveCountry', countryDetail);
    };

    var DeleteCountry = function (countryId) {
        return $http.get(config.SERVICE_URL + '/Country/DeleteCountry',
            {
                params: {
                    countryId: countryId
                }
            });
    };

    var DeleteCountryDocument = function (countryDocumentId) {
        return $http.get(config.SERVICE_URL + '/Country/DeleteCountryDocument',
            {
                params: {
                    countryDocumentId: countryDocumentId
                }
            });
    };

    var GetCountryDetail = function (countryId) {
        return $http.get(config.SERVICE_URL + '/Country/GetCountryDetail',
            {
                params: {
                    countryId: countryId
                }
            });
    };

    return {
        SaveCountry: SaveCountry,
        GetCountyDocumentList: GetCountyDocumentList,
        GetCountryList: GetCountryList,
        GetCountryCodeList: GetCountryCodeList,
        GetAllCountryList: GetAllCountryList,
        DeleteCountry: DeleteCountry,
        DeleteCountryDocument: DeleteCountryDocument,
        GetCountryDetail: GetCountryDetail,
        DeleteCountryHoliday: DeleteCountryHoliday
    };

});