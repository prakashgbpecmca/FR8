/**
 * Service
 */
angular.module('ngApp.weekdays').factory('WeekDaysService', function ($http, config, SessionService) {
    
    var GetWeekDays = function (id) {
        return $http.get(config.SERVICE_URL + '/WeekDays/GetWeekDays',
            {
                params: {
                    weekDayId: id
                }
            });
    };

    var GetWeekDaysList = function () {

        return $http.get(config.SERVICE_URL + '/WeekDays/GetWeekDaysList');
    };
    
    var SaveWeekDay = function (weekData) {
        return $http.post(config.SERVICE_URL + '/WeekDays/SaveWeekDay', weekData);
    };

    var DeleteWorkingWeekDay = function (id) {
        return $http.get(config.SERVICE_URL + '/WeekDays/DeleteWorkingWeekDay',
            {
                params: {
                    weekDayId: id
                }
            });
    };
    return {
        GetWeekDays: GetWeekDays,
        GetWeekDaysList: GetWeekDaysList,
        SaveWeekDay: SaveWeekDay,
        DeleteWorkingWeekDay: DeleteWorkingWeekDay
    };
});