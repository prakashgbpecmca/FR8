/**
 * Service
 */
angular.module('ngApp.agent').factory('AgentService', function ($http, config, SessionService) {

    var GetAgentList = function () {
        return $http.get(config.SERVICE_URL + '/Agent/GetAgentList');
    };

    var SaveAgent = function (carrierDetail) {
        return $http.post(config.SERVICE_URL + '/Agent/SaveAgent', carrierDetail);
    };

    var DeleteAgent = function (agentId) {
        return $http.get(config.SERVICE_URL + '/Agent/DeleteAgent',
            {
                params: {
                    agentId: agentId
                }
            });
    };

    var GetAgents = function () {
        return $http.get(config.SERVICE_URL + '/Agent/GetAgents');
    };

    var GetCountryAgents = function (countryId) {
        return $http.get(config.SERVICE_URL + '/Agent/GetAgents',
            {
                params: {
                    countryId: countryId
                }
            });
    };

    var GetAgentDetail = function (agentId) {
        return $http.get(config.SERVICE_URL + '/Agent/GetAgentDetail',
            {
                params: {
                    agentId: agentId
                }
            });
    };


    return {
        SaveAgent: SaveAgent,
        GetAgentList: GetAgentList,
        DeleteAgent: DeleteAgent,
        GetAgents: GetAgents,
        GetAgentDetail: GetAgentDetail,
        GetCountryAgents: GetCountryAgents
    };

});