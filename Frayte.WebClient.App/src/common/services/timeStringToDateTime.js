angular.module('ngApp.common')
.factory("TimeStringtoDateTime", function () {
    var ConvertString = function (DateTime, Time) {
        var Timeformat = moment(Time, "hmmss").format("HH:mm:ss");
        var TimeArray = Timeformat.split(':');
        DateTime.setHours(TimeArray[0]);
        DateTime.setMinutes(TimeArray[1]);
        DateTime.setSeconds(TimeArray[2]);
        return DateTime;
    };

    var ConvertTimeString = function (Time) {
        var DateTime = new Date();
        var Timeformat = moment(Time, "hmmss").format("HH:mm:ss");
        var TimeArray = Timeformat.split(':');
        DateTime.setHours(TimeArray[0]);
        DateTime.setMinutes(TimeArray[1]);
        DateTime.setSeconds("00");
        var UtcDate = moment.utc(DateTime).toDate();
        return DateTime;
    };

    return {
        ConvertString: ConvertString,
        ConvertTimeString: ConvertTimeString
    };

});


