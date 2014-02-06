using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    public interface IAgents
    {
        Task<List<Agent>> All();
    }
}