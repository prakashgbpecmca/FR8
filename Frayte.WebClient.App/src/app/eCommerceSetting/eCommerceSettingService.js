angular.module('ngApp.ecommerceSetting').factory('ecommerceSettingService', function ($http, config, SessionService) {

    var GetCountryList = function () {
        return $http.get(config.SERVICE_URL + '/eCommerceSetting/CountryList');
    };
    var AddEditHSCode = function (obj) {
        return $http.post(config.SERVICE_URL + '/eCommerceSetting/AddEditHSCodeSetting', obj);
    };

    var GetHSCodeDetailList = function (countryId) {
        return $http.get(config.SERVICE_URL + '/eCommerceSetting/GeteCommerceHSCodeDetail',
            {
                params: {
                    countryId: countryId
                }
            });
    };

    var DeleteHSCodeSetting = function (HSCodeId) {
        return $http.get(config.SERVICE_URL + '/eCommerceSetting/DeleteHSCodeSetting',
          {
              params: {
                  HSCodeId: HSCodeId
              }
          });
    };
    return {
        GetCountryList: GetCountryList,
        GetHSCodeDetailList: GetHSCodeDetailList,
        AddEditHSCode: AddEditHSCode,
        DeleteHSCodeSetting: DeleteHSCodeSetting
    };
});