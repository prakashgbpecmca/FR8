angular.module('ngApp.common')
.factory("DateFormatChange", function ($filter) {
    var dateFormatChange = function (CountriesStore) {
        var dateStr = CountriesStore;
        var date = new Date(dateStr);
        var days = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        var getmn1 = days[date.getMonth()];
        var getyr = date.getFullYear();
        var getday = date.getDate();
        var strDate = getday +"-"+getmn1 + "-" + getyr;
        return strDate;

        

        
    };

    return {
        DateFormatChange: dateFormatChange
    };

});


