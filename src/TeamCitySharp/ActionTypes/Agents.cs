using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    internal class Agents : IAgents
    {
        private readonly TeamCityCaller _caller;

        internal Agents(TeamCityCaller caller)
        {
            _caller = caller;
        }

        public async Task<List<Agent>> All()
        {
            var agentWrapper = await _caller.Get<AgentWrapper>("/app/rest/agents");
            return agentWrapper.Agent;
        }
    }
}