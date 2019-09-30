/**
 * Service
 */
angular.module('ngApp.utility').factory('UtilityService', function ($http, config, $state, SessionService) {

    var GetCurrentRoute = function (tabs, searchTerm) {

        if (tabs!== null && tabs !== undefined) {
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

                    if (search === "booking-home.direct-booking-clone" || search === "booking-home.eCommerce-booking-clone") {
                        CallingType = 'ShipmentClone';
                        break;
                    }
                    else if (search === "booking-home.direct-booking-return" || search === "booking-home.eCommerce-booking-return") {
                        CallingType = 'ShipmentReturn';
                        break;
                    }
                    else if (search === "booking-home.direct-booking" || search === "booking-home.eCommerce-booking") {
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
                return "frayte.com";
            }
            else {
                return "frayte.co.uk";
            }
        }

    };
    return {
        getPublicSiteName:getPublicSiteName,
        GetCurrentRoute: GetCurrentRoute,
        ShipmentType: ShipmentType,
        GetCurrentShipmentType: GetCurrentShipmentType
    };

});