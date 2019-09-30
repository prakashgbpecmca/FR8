angular.module('ngApp.common', ['pascalprecht.translate'])
.factory("SessionService", function ($window, $state) {
    function setUser(userInfo) {
        $window.sessionStorage["userInfo"] = JSON.stringify(userInfo);
    }
    function getUser(userInfo) {
        return $window.sessionStorage["userInfo"] ? JSON.parse($window.sessionStorage["userInfo"]) : undefined;
    }
    function removeUser() {
        $window.sessionStorage.removeItem("userInfo");
    }
    function setLanguage(lang) {
        $window.sessionStorage["language"] = lang;
    }
    function getLanguage() {
        return $window.sessionStorage["language"] ? $window.sessionStorage["language"] : 'en';
    }
    function getScreenHeight() {
        var screenHeight = null;
        screenHeight = $window.outerHeight;
        if (screenHeight < 601) {
            return 360;
        }
        else if (screenHeight < 769 && screenHeight > 600) {
            return 402;
        }
        else if (screenHeight < 801 && screenHeight > 768) {
            return 462;
        }
        else if (screenHeight < 901 && screenHeight > 800) {
            return 520;
        }
        else if (screenHeight < 1051 && screenHeight > 900) {
            return 588;
        }
        else if (screenHeight < 1081 && screenHeight > 1050) {
            return 750;
        }
        else if (screenHeight < 1201 && screenHeight > 1080) {
            return 820;
        }
        else {
            return 402;
        }
    }

    function windowUnload(evt) {
        if ($state.current.name === 'customer.booking-home.eCommerce-booking' ||
            $state.current.name === 'admin.booking-home.direct-booking' ||
              $state.current.name === 'customer.booking-home.direct-booking' ||
             $state.current.name === 'admin.booking-home.direct-booking' ||
              $state.current.name === 'dbuser.booking-home.direct-booking') {
            var message = 'Are you sure you want to leave?';
            if (typeof evt == 'undefined') {
                evt = window.event;
            }
            if (evt) {
                evt.returnValue = message;
            }
            return message;
        }
        else {

        }

    }
    return {
        setUser: setUser,
        getUser: getUser,
        removeUser: removeUser,
        getScreenHeight: getScreenHeight,
        setLanguage: setLanguage,
        getLanguage: getLanguage,
        windowUnload: windowUnload
    };

})
.factory("Spinner", function () {

    function show(userInfo) {
        document.getElementById('loader').style.display = 'block';
    }
    function hide(userInfo) {
        document.getElementById('loader').style.display = 'none';
    }

    return {
        show: show,
        hide: hide
    };
})
.filter('timeStampFilter', function () {
    return function (value) {
        var localTime = moment.utc(value).toDate();
        localTime = moment(localTime).format('LT');
        return localTime;
    };
})

.filter('dateFilter', function () {
    return function (value) {
        if (value === undefined || value === null) {
            return '';
        }
        else {
            var localTime = moment.utc(value).toDate();
            localTime = moment(localTime).format('DD/MM/YYYY');
            return localTime;
        }
    };
})

.filter('shortTimeFilter', function () {
    return function (value) {
        if (value === undefined || value === null) {
            return '';
        }
        else {
            var hh = value.substring(0, 2);
            var mm = value.substring(2, 4);
            return hh + ':' + mm;
        }
    };
});