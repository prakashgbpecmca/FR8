/**
 * Service
 */
angular.module('ngApp.user').factory('UtilityService', function ($http, DateFormatChange, $filter, config, $state, SessionService) {

    var FrayteSystemRoles = {
        Admin: 1,
        Agent: 2,
        Customer: 3,
        Receiver: 4,
        Shipper: 5,
        Staff: 6,
        Warehouse: 7,
        HSCodeOperator: 8,
        MasterAdmin: 9,
        WarehouseAgent: 10,
        HSCodeOperatorManager: 11,
        CallCenterOperator: 12,
        CallCenterManger: 13,
        Accountant: 14,
        Consolidator: 15
    };

    var GetCurrentParentExactRoute = function (tabs, searchTerm) {
        if (tabs !== null && tabs !== undefined) {
            for (var i = 0; i < tabs.length; i++) {
                if (tabs[i].route === searchTerm) {
                    return tabs[i].route;
                }
            }
        }
        return "";
    };
    var GetCurrentRoute = function (tabs, searchTerm) {

        if (tabs !== null && tabs !== undefined) {
            for (var i = 0; i < tabs.length; i++) {
                var str = tabs[i].route;
                var arr = str.split(".");
                var array = arr.splice(0, 1);
                var search = "";
                if (arr !== null && arr.length) {
                    for (var j = 0; j < arr.length; j++) {
                        search += arr[j];
                        if (j !== arr.length - 1) {
                            search += ".";
                        }
                    }

                    if (search === searchTerm) {
                        return tabs[i].route;
                    }
                }
            }
        }

        return "";
    };

    var GetCurrentParentRoute = function (tabs, searchTerm) {

        for (var k = 0; k < tabs.length; k++) {
            var tabRoute = tabs[k].route;
            var arr = tabRoute.split(".");
            arr.splice(0, 1);
            var search = "";
            if (arr !== null && arr.length) {
                for (var j = 0; j < arr.length; j++) {
                    search += arr[j];
                    if (j !== arr.length - 1) {
                        search += ".";
                    }
                }

                if (search === searchTerm) {
                    return tab.childTabs[i].route;
                }
            }
        }

        return "";
    };
    var GetCurrentShipmentType = function (tab) {

        var CallingType = "";
        for (var i = 0; i < tab.childTabs.length; i++) {

            if ($state.current.name === tab.childTabs[i].route) {
                var str = tab.childTabs[i].route;
                var arr = str.split(".");
                var search = "";
                arr.splice(0, 1);
                if (arr !== null && arr.length) {
                    for (var j = 0; j < arr.length; j++) {
                        search += arr[j];
                        if (j !== arr.length - 1) {
                            search += ".";
                        }
                    }

                    if (search === "userTabs.direct-booking-clone" || search === "userTabs.eCommerce-booking-clone") {
                        CallingType = 'ShipmentClone';
                        break;
                    }
                    else if (search === "userTabs.direct-booking-return" || search === "userTabs.eCommerce-booking-return") {
                        CallingType = 'ShipmentReturn';
                        break;
                    }
                    else if (search === "userTabs.direct-booking" || search === "userTabs.eCommerce-booking") {
                        CallingType = 'ShipmentDraft';
                        break;
                    }
                    else {
                        CallingType = '';
                    }
                }
            }

        }

        return CallingType;
    };

    var ShipmentType = {
        ShipmentReturn: "ShipmentReturn",
        ShipmentClone: "ShipmentClone",
        ShipmentDraft: "ShipmentDraft"
    };

    var ShowFormsError = function () {

    };
    var getPublicSiteName = function () {
        if (config.Public_Link.search("localhost") === -1) {
            if (config.Public_Link.search("https") > -1) {
                return config.Public_Link.replace("https://", "").replace("/", "");
            }
            else if (config.Public_Link.search("http") > -1) {
                return config.Public_Link.replace("http://", "").replace("/", "");
            }
        }
        else {
            if (config.SITE_COUNTRY === "COM") {
                return "www.FRAYTE.com";
            }
            else {
                return "www.FRAYTE.co.uk";
            }
        }

    };

    var eCommerceShipmentType = {
        eCommerceONL: "ECOMMERCE_ONL",
        eCommerceWS: "ECOMMERCE_WS",
        eCommerceSS: "ECOMMERCE_SS"
    };

    var GetShipmentChargeableWeight = function (moduleType) {

    };
    var GetShipemntTotalWeight = function () {

    };

    var GetForMattedDate = function (date) {
        // Geting Correct Date Format
        if (date !== null && date !== '' && date !== undefined) {
            var newDate = moment.utc(date).toDate();
            Number.prototype.padLeft = function (base, chr) {
                var len = (String(base || 10).length - String(this).length) + 1;
                return len > 0 ? new Array(len).join(chr || '0') + this : this;
            };

            var d = newDate;

            var dformat = DateFormatChange.DateFormatChange(d);
            return dformat;
        }
        else {
            return;
        }
    };

    var GetFormattedtime = function (time) {
        var formattedTime = "";
        if (time) {
            var arr = time.split("");
            if (time.length === 3) {
                formattedTime = "0";
                for (var i = 0 ; i < arr.length; i++) {
                    if (i === 1) {
                        formattedTime += ":";
                    }
                    formattedTime += arr[i];
                }
            } else {
                for (var j = 0 ; j < arr.length; j++) {
                    if (j === 2) {
                        formattedTime += ":";
                    }
                    formattedTime += arr[j];
                }
            }
        }
        return formattedTime;
    };

    var UserEmailValidity = function (email, userType) {
        return $http.get(config.SERVICE_URL + "/Account/IsEmailExist", {
            params: {
                email: email,
                userType: userType
            }
        });
    };
    var GetUnmanifestedJobCount = function (userId) {
        return $http.get(config.SERVICE_URL + "/Manifest/GetUnmanifestedJobCount", {
            params: {
                userId: userId
            }
        });
    };

    var getUserTabs = function (userId, roleId, moduleType) {
        return $http.get(config.SERVICE_URL + '/Login/GetUserTabs', {
            params: {
                userId: userId,
                roleId: roleId,
                moduleType: moduleType
            }
        });

    };
   
    var getLogisticServices = function (operationZoneId) {

        return $http.get(config.SERVICE_URL + "/LogisticItem/LogisticServices", {
            params: {
                operationZoneId: operationZoneId
            }
        });
    };


    var getLogisticTypesByOperationZone = function (logisticServices, operationZoneId) {
        if (logisticServices && operationZoneId) {
            var logisticTypes = [];
            angular.forEach(logisticServices, function (obj) {
                if (operationZoneId === obj.OperationZoneId) {

                    var obj1 = {
                        Display: '',
                        Value: ''
                    };

                    var found = [];
                    for (var i = 0; i < logisticTypes.length; i++) {
                        if (logisticTypes[i].Value === obj.LogisticType) {
                            found.push(logisticTypes[i].Value);
                        }
                    }

                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.LogisticType;
                        obj1.Display = obj.LogisticTypeDisplay;
                        if (obj1.Value && obj1.Display) {
                            logisticTypes.push(obj1);
                        }
                    }
                }
            });
            return logisticTypes.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }
    };
    var getLogisticCompaniesByLogisticType = function (logisticServices, logisticType) {
        if (logisticServices && logisticType) {
            var courierCompanies = [];
            angular.forEach(logisticServices, function (obj) {
                if (logisticType === obj.LogisticType) {

                    var obj1 = {
                        Display: '',
                        Value: ''
                    };
                    var found = [];
                    for (var i = 0; i < courierCompanies.length; i++) {
                        if (courierCompanies[i].Value === obj.LogisticCompany) {
                            found.push(courierCompanies[i].Value);
                        }
                    }

                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.LogisticCompany;
                        obj1.Display = obj.LogisticCompanyDisplay;
                        if (obj1.Value && obj1.Display) {
                            courierCompanies.push(obj1);
                        }
                    }

                }
            });
            return courierCompanies.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }
    };
    var getRateTypesByCourierCompany = function (logisticServices, logisticComapny, logisticType) {
        if (logisticServices && logisticComapny) {
            var rateTypes = [];
            angular.forEach(logisticServices, function (obj) {
                if (logisticComapny === obj.LogisticCompany && logisticType === obj.LogisticType) {
                    var obj1 = {
                        Display: '',
                        Value: ''
                    };
                    var found = [];
                    for (var i = 0; i < rateTypes.length; i++) {
                        if (rateTypes[i].Value === obj.RateType) {
                            found.push(rateTypes[i].Value);
                        }
                    }

                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.RateType;
                        obj1.Display = obj.RateTypeDisplay;

                        if (obj1.Value && obj1.Display) {
                            rateTypes.push(obj1);
                        }
                    }
                }
            });
            return rateTypes.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }
    };
    var getRateCardLogisticService = function (operationZoneId) {

        return $http.get(config.SERVICE_URL + "/LogisticItem/RateCardLogisticServices", {
            params: {
                operationZoneId: operationZoneId
            }
        });
    };
    var getRateCardLogisticTypesByOperationZone = function (logisticServices, operationZoneId) {
        if (logisticServices && operationZoneId) {
            var logisticTypes = [];
            angular.forEach(logisticServices, function (obj) {
                if (operationZoneId === obj.OperationZoneId) {

                    var obj1 = {
                        Display: '',
                        Value: ''
                    };

                    var found = [];
                    for (var i = 0; i < logisticTypes.length; i++) {
                        if (logisticTypes[i].Value === obj.LogisticType) {
                            found.push(logisticTypes[i].Value);
                        }
                    }

                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.LogisticType;
                        obj1.Display = obj.LogisticTypeDisplay;
                        if (obj1.Value && obj1.Display) {
                            logisticTypes.push(obj1);
                        }
                    }
                }
            });
            return logisticTypes.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }
    };
    var getRateCardLogisticCompaniesByLogisticType = function (logisticServices, logisticType) {
        if (logisticServices && logisticType) {
            var courierCompanies = [];
            angular.forEach(logisticServices, function (obj) {
                if (logisticType === obj.LogisticType) {

                    var obj1 = {
                        Display: '',
                        Value: ''
                    };
                    var found = [];
                    for (var i = 0; i < courierCompanies.length; i++) {
                        if (courierCompanies[i].Value === obj.LogisticCompany) {
                            found.push(courierCompanies[i].Value);
                        }
                    }
                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.LogisticCompany;
                        obj1.Display = obj.LogisticCompanyDisplay;
                        if (obj1.Value && obj1.Display) {
                            courierCompanies.push(obj1);
                        }
                    }

                }
            });
            return courierCompanies.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }
    };
    var getRateCardRateTypesByLogisticCompany = function (logisticServices, logisticComapny, logisticType) {
        if (logisticServices && logisticComapny && logisticType) {
            var rateTypes = [];
            angular.forEach(logisticServices, function (obj) {
                if (logisticComapny === obj.LogisticCompany && logisticType === obj.LogisticType) {
                    var obj1 = {
                        Display: '',
                        Value: ''
                    };
                    var found = [];
                    for (var i = 0; i < rateTypes.length; i++) {
                        if (rateTypes[i].Value === obj.RateType) {
                            found.push(rateTypes[i].Value);
                        }
                    }
                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.RateType;
                        obj1.Display = obj.RateTypeDisplay;

                        if (obj1.Value && obj1.Display) {
                            rateTypes.push(obj1);
                        }
                    }
                }
            });
            return rateTypes.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }
    };
    var getRateCardShipemntTypes = function (logisticServices, logisticComapny, logisticType, rateType) {
        if (logisticServices && logisticComapny && logisticType) {
            var shipmentTypes = [];
            angular.forEach(logisticServices, function (obj) {
                if (logisticComapny === obj.LogisticCompany && logisticType === obj.LogisticType && rateType === obj.RateType) {
                    var obj1 = {
                        Display: '',
                        Value: ''
                    };

                    var found = [];

                    for (var i = 0; i < shipmentTypes.length; i++) {
                        if (shipmentTypes[i].Value === obj.DocType) {
                            found.push(shipmentTypes[i].Value);
                        }
                    }

                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.DocType;
                        obj1.Display = obj.DocTypeDisplay;

                        if (obj1.Value && obj1.Display) {
                            shipmentTypes.push(obj1);
                        }
                    }
                }
            });
            return shipmentTypes.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }

    };
    var getRateCardParcelTypes = function (logisticServices, logisticComapny, logisticType) {

        if (logisticServices && logisticComapny && logisticType) {
            var parcelTypes = [];
            angular.forEach(logisticServices, function (obj) {
                if (logisticComapny === obj.LogisticCompany && logisticType === obj.LogisticType) {
                    var obj1 = {
                        Display: '',
                        Value: ''
                    };
                    var found = [];

                    for (var i = 0; i < parcelTypes.length; i++) {
                        if (parcelTypes[i].Value === obj.ParcelType) {
                            found.push(parcelTypes[i].Value);
                        }
                    }

                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.ParcelType;
                        obj1.Display = obj.ParcelTypeDisplay;

                        if (obj1.Value && obj1.Display) {
                            parcelTypes.push(obj1);
                        }
                    }
                }
            });
            return parcelTypes.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }
    };
    var getRateCardPackageTypes = function (logisticServices, logisticComapny, logisticType) {
        if (logisticServices && logisticComapny && logisticType) {
            var packageTypes = [];
            angular.forEach(logisticServices, function (obj) {
                if (logisticComapny === obj.LogisticCompany && logisticType === obj.LogisticType) {
                    var obj1 = {
                        Display: '',
                        Value: ''
                    };
                    var found = [];

                    for (var i = 0; i < packageTypes.length; i++) {
                        if (packageTypes[i].Value === obj.PackageType) {
                            found.push(packageTypes[i].Value);
                        }
                    }

                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.PackageType;
                        obj1.Display = obj.PackageTypeDisplay;

                        if (obj1.Value && obj1.Display) {
                            packageTypes.push(obj1);
                        }
                    }
                }
            });
            return packageTypes.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }
    };
    var getRateCardAddressTypes = function (logisticServices, logisticComapny, logisticType) {
        if (logisticServices && logisticComapny && logisticType) {
            var addressTypes = [];
            angular.forEach(logisticServices, function (obj) {
                if (logisticComapny === obj.LogisticCompany && logisticType === obj.LogisticType) {
                    var obj1 = {
                        Display: '',
                        Value: ''
                    };
                    var found = [];
                    for (var i = 0; i < addressTypes.length; i++) {
                        if (addressTypes[i].Value === obj.AddressType) {
                            found.push(addressTypes[i].Value);
                        }
                    }
                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.AddressType;
                        obj1.Display = obj.AddressTypeDisplay;

                        if (obj1.Value && obj1.Display) {
                            addressTypes.push(obj1);
                        }
                    }
                }
            });
            return addressTypes.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }

    };
    var getRateCardServiceTypes = function (logisticServices, logisticComapny, logisticType) {
        if (logisticServices && logisticComapny && logisticType) {
            var serviceTypes = [];
            angular.forEach(logisticServices, function (obj) {
                if (logisticComapny === obj.LogisticCompany && logisticType === obj.LogisticType) {
                    var obj1 = {
                        Display: '',
                        Value: ''
                    };
                    var found = [];
                    for (var i = 0; i < serviceTypes.length; i++) {
                        if (serviceTypes[i].Value === obj.ServiceType) {
                            found.push(serviceTypes[i].Value);
                        }
                    }
                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.ServiceType;
                        obj1.Display = obj.ServiceTypeDisplay;

                        if (obj1.Value && obj1.Display) {
                            serviceTypes.push(obj1);
                        }
                    }
                }
            });
            return serviceTypes.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }

    };
    var getRateCardPODTypes = function (logisticServices, logisticComapny, logisticType) {

        if (logisticServices && logisticComapny && logisticType) {
            var podTypes = [];
            angular.forEach(logisticServices, function (obj) {
                if (logisticComapny === obj.LogisticCompany && logisticType === obj.LogisticType) {
                    var obj1 = {
                        Display: '',
                        Value: ''
                    };
                    var found = [];
                    for (var i = 0; i < podTypes.length; i++) {
                        if (podTypes[i].Value === obj.PODType) {
                            found.push(podTypes[i].Value);
                        }
                    }
                    if (found.length) {
                    }
                    else {
                        obj1.Value = obj.PODType;
                        obj1.Display = obj.PODTypeDisplay;

                        if (obj1.Value && obj1.Display) {
                            podTypes.push(obj1);
                        }
                    }
                }
            });
            return podTypes.sort(function (a, b) {
                if (a.Value.toLowerCase() < b.Value.toLowerCase()) {
                    return -1;
                }
                if (a.Value.toLowerCase() > b.Value.toLowerCase()) {
                    return 1;
                }

                return 0;
            });
        }

    };
    var getTradelaneFrayteRef = function (FrayteNumber) {
        if (FrayteNumber) {
            return FrayteNumber.replace("TL", "");
        }
        else {
            return "";
        }
    };

    return {
        FrayteSystemRoles:FrayteSystemRoles,
        getTradelaneFrayteRef: getTradelaneFrayteRef,
        getPublicSiteName: getPublicSiteName,
        getRateCardPODTypes: getRateCardPODTypes,
        getRateCardAddressTypes: getRateCardAddressTypes,
        getRateCardServiceTypes: getRateCardServiceTypes,
        getRateCardPackageTypes: getRateCardPackageTypes,
        getRateCardParcelTypes: getRateCardParcelTypes,
        getRateCardShipemntTypes: getRateCardShipemntTypes,
        getRateCardRateTypesByLogisticCompany: getRateCardRateTypesByLogisticCompany,
        getRateCardLogisticCompaniesByLogisticType: getRateCardLogisticCompaniesByLogisticType,
        getRateCardLogisticTypesByOperationZone: getRateCardLogisticTypesByOperationZone,
        getRateCardLogisticService: getRateCardLogisticService,
        getLogisticTypesByOperationZone: getLogisticTypesByOperationZone,
        getLogisticCompaniesByLogisticType: getLogisticCompaniesByLogisticType,
        getRateTypesByCourierCompany: getRateTypesByCourierCompany,
        getLogisticServices: getLogisticServices,
        GetCurrentParentExactRoute: GetCurrentParentExactRoute,
        eCommerceShipmentType: eCommerceShipmentType,
        GetCurrentRoute: GetCurrentRoute,
        ShipmentType: ShipmentType,
        GetShipmentChargeableWeight: GetShipmentChargeableWeight,
        GetShipemntTotalWeight: GetShipemntTotalWeight,
        GetCurrentShipmentType: GetCurrentShipmentType,
        ShowFormsError: ShowFormsError,
        GetForMattedDate: GetForMattedDate,
        GetFormattedtime: GetFormattedtime,
        UserEmailValidity: UserEmailValidity,
        GetUnmanifestedJobCount: GetUnmanifestedJobCount,
        getUserTabs: getUserTabs
    };

});