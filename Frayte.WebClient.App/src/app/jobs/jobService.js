angular.module('ngApp.hsCodejobs').factory('JobService', function ($http, config, SessionService) {
    var GetJobsDetails = function (userId) {
        return $http.get(config.SERVICE_URL + "/HSCodeJob/GetJobsDetails", {
            params: {
                userId: userId
            }
        });
    };

    var GetUnAssignedJobs = function (obj) {
        return $http.post(config.SERVICE_URL + '/HSCodeJob/GetUnAssignedJobs', obj);
    };
    var OperatorsWithJobs = function (userId) {
        return $http.get(config.SERVICE_URL + "/HSCodeJob/GetOperatorsWithJobs", {
            params: {
                userId: userId
            }
        });
    };
    var MangerOperators = function (userId) {
        return $http.get(config.SERVICE_URL + "/HSCodeJob/GetMangerOperators", {
            params: {
                userId: userId
            }
        });
    };

    var AssignJobsToOperator = function (jobs) {
        return $http.post(config.SERVICE_URL + "/HSCodeJob/AsssignJobs", jobs);
    };

    var GetAssignedJobs = function (obj) {
        return $http.post(config.SERVICE_URL + '/HSCodeJob/GetAssignedJobs', obj);
    };

    var SetShipmentHSCode = function (eCommerceShipmentDetailid, HSCode, Description) {

        return $http.get(config.SERVICE_URL + '/HSCodeJob/SetShipmentHSCode',
        {
            params: {
                eCommerceShipmentDetailid: eCommerceShipmentDetailid,
                HSCode: HSCode,
                Description: Description
            }
        });
    };

    var SetShipmentHSCodeNew = function (eCommerceShipmentDetailid, HSCode, Description, HsCodeId) {

        return $http.get(config.SERVICE_URL + '/HSCodeJob/SetShipmentHSCode',
        {
            params: {
                eCommerceShipmentDetailid: eCommerceShipmentDetailid,
                HSCode: HSCode,
                Description: Description,
                HsCodeId: HsCodeId
            }
        });
    };

    var ReAssignJobs = function (jobs) {
        return $http.post(config.SERVICE_URL + '/HSCodeJob/ReAssignJobs', jobs);
    };
    var GetDestinationCountries = function (userId) {
        return $http.get(config.SERVICE_URL + '/HSCodeJob/GetDestinationCountries',
      {
          params: {
              userId: userId
          }
      });
    };
    var GetJobsInProgressCount = function () {
        return $http.get(config.SERVICE_URL + '/HSCodeJob/GetJobsInProgressCount');
    };
    var AvgJobsPerOperatorPerHour = function (userId) {
        return $http.get(config.SERVICE_URL + '/HSCodeJob/HSCodeOutputPerOperatorPerHour',
   {
       params: {
           userId: userId
       }
   });
    };
    return {
        GetJobsDetails: GetJobsDetails,
        GetUnAssignedJobs: GetUnAssignedJobs,
        OperatorsWithJobs: OperatorsWithJobs,
        MangerOperators: MangerOperators,
        AssignJobsToOperator: AssignJobsToOperator,
        GetAssignedJobs: GetAssignedJobs,
        SetShipmentHSCode: SetShipmentHSCode,
        SetShipmentHSCodeNew: SetShipmentHSCodeNew,
        ReAssignJobs: ReAssignJobs,
        GetDestinationCountries: GetDestinationCountries,
        GetJobsInProgressCount: GetJobsInProgressCount,
        AvgJobsPerOperatorPerHour: AvgJobsPerOperatorPerHour
    };
});